#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Main Dash app entry point - interactive situational awareness visualisation and analysis framework

This module is a web-based application powered by Dash and Plotly.py, both of which are essential data analysis,
visualisation, and hosting libraries. Dash is the library to build the web app which contains interactive plots that are
drawn with Plotly.py

The web app expects two input files from the Dash file uploader:

* CSV: Live data recorded at real-time, which contains coordinates, distances, game objects, etc.
* TXT: Summary data containing the frame slicing number for each room, and the time player uses in each room.

"""

import base64
import io
from datetime import datetime

import dash
import pandas as pd
import plotly.express as px
from plotly.graph_objects import Figure
from dash import dash_table, dcc, html
from dash.dependencies import Input, Output, State

from utils.plot_3d import coords_concat, duration_parser

external_stylesheets = [
    "https://unpkg.com/tailwindcss@^2/dist/tailwind.min.css",
]

app = dash.Dash(__name__, external_stylesheets=external_stylesheets)
app.layout = html.Div(
    [
        html.Header(
            className="text-gray-600 body-font",
            children=html.Div(
                [
                    html.A(
                        [
                            html.Img(src="https://img.icons8.com/color/96/000000/cosine.png", className="h-10 w-10"),
                            html.Span("Visualisation and Analysis", className="ml-3 text-xl"),
                        ],
                        href="/",
                        className="flex title-font font-medium items-center text-gray-900 mb-4 md:mb-0",
                    ),
                    dcc.Upload(
                        id="upload-csv",
                        children=html.Button(
                            "Upload file",
                            className="inline-flex items-center bg-gray-100 border-0 py-1 px-3 focus:outline-none "
                            "hover:bg-gray-200 rounded text-base mt-4 md:mt-0",
                        ),
                        multiple=True,
                    ),
                ],
                className="container mx-auto flex flex-wrap p-5 flex-col md:flex-row items-center justify-between",
            ),
        ),
        html.Div(
            dcc.Loading(html.Div(id="output-visualise"), color="#80CBC4", type="cube"),
            className="container mx-auto p-5 flex flex-1 justify-center items-center",
        ),
        html.Footer(
            [
                html.P("Situational Awareness Visualisation and Analysis"),
                html.I("Built with ❤️ at UofG ©️ 2021 - Shangbo Wu"),
            ],
            className="p-5 text-sm text-gray-400 bg-gray-50 text-center",
        ),
    ],
    className="h-screen flex flex-col",
)


def scatter_figure(
    coords: pd.DataFrame, title: str, img: str, img_x: float, img_y: float, img_sizex: float, img_sizey: float
) -> Figure:
    """Extracts the three coordinates from the dataframe and create a 2D scatter plot, which is overlayed on the top of
    the top-down view of the game scene with the image provided as an external URL.

    Args:
        coords (pd.DataFrame): The coordinate dataframe, which contains x, y, z, and type (user/cam/controller).
        title (str): The title of the plot.
        img (str): The absolute external URL of the image.
        img_x (float): Background image top right x anchor point.
        img_y (float): Background image top right y anchor point.
        img_sizex (float): The x side length of the image.
        img_sizey (float): The y side length of the image.

    Returns:
        Figure: A Plotly.py figure instance.

    """
    fig = px.scatter(coords, x="x", y="y", color="type", width=500, height=580, title=title, opacity=0.6)
    fig.update_layout(
        legend=dict(yanchor="bottom", y=1.02, xanchor="right", x=1, itemsizing="constant"),
        legend_title_text=None,
        template="plotly_white",
    )
    fig.add_layout_image(
        dict(
            source=img,
            xref="x",
            yref="y",
            x=img_x,
            y=img_y,
            sizex=img_sizex,
            sizey=img_sizey,
            sizing="stretch",
            opacity=0.8,
            layer="below",
        )
    )
    return fig


def interactive_3d_figure(coords: pd.DataFrame, title: str) -> Figure:
    """Interactive 3D plots of the coordinates (user/cam/controller)

    Args:
        coords (pd.DataFrame): Coordinates dataframe containing x, y, z, and type.
        title (str): The title of the 3D plot.

    Returns:
        Figure: A Plotly.py figure instance.

    """
    fig = px.scatter_3d(
        coords, x="x", y="y", z="z", color="type", symbol="type", opacity=0.6, width=560, height=560, title=title
    )
    fig.update_layout(
        legend=dict(yanchor="bottom", y=1.02, xanchor="right", x=1, itemsizing="constant"), legend_title_text=None,
    )
    fig.update_traces(marker={"size": 3})
    return fig


def aggregate_attention_obj(df: pd.DataFrame, column_name: str) -> pd.DataFrame:
    """Aggregate the number of game objects by their names - used for plotting histograms later.

    Args:
        df (pd.DataFrame): The main dataframe containing all live data collected.
        column_name (str): The name of the game object column.

    Returns:
        pd.DataFrame: An aggregated dataframe containing the number of each unique game object.

    """
    return (
        df.groupby(column_name)
        .count()
        .reset_index()
        .rename(columns={"frame_no": "count"})
        .sort_values(["count"], ascending=False)
    )


def parse_uploaded_file(contents: list[str, str], filename: list[str, str], date: list[str, str]) -> html.Div:
    """Expects two files from the upload: a CSV file and a TXT file.

    Args:
        contents (list[str, str]): Base64 encodings of the two file contents.
        filename (list[str, str]): The names of the two files, one ending with .csv, and the other ending with .txt.
        date (list[str, str]): The last modified date of the two files respectively.

    Returns:
        html.Div: Dash HTML component, renders the web page contents.

    """
    _, content_string_0 = contents[0].split(",")
    _, content_string_1 = contents[1].split(",")
    decoded_0 = base64.b64decode(content_string_0)
    decoded_1 = base64.b64decode(content_string_1)

    # Parse the two uploaded files and determining whether the contents are of the right specification
    try:
        if "csv" in filename[0] and "txt" in filename[1]:
            df = pd.read_csv(io.StringIO(decoded_0.decode("utf-8")))
            txt_content = decoded_1.decode("utf-8")
            csv_filename, txt_filename = filename
            csv_date, txt_date = date
        elif "csv" in filename[1] and "txt" in filename[0]:
            df = pd.read_csv(io.StringIO(decoded_1.decode("utf-8")))
            txt_content = decoded_0.decode("utf-8")
            txt_filename, csv_filename = filename
            txt_date, csv_date = date

    except Exception as e:
        print(e)
        return html.Div(["There was an error parsing the file."])

    # Sample of the summary file contains:
    #
    # - Room 1 Time: 0:00:49.886678   -> The time for the player to clear room 1
    # - Room 1 Start Frame: 0         -> The frame number when the player started room 1
    #
    # A total of three rooms are logged in the summary text, the following section parse the time and frames from it.
    summary = [line.split(":", 1)[-1] for line in txt_content.splitlines()]
    time_spent = [summary[0], summary[2], summary[4]]
    time_spent = pd.DataFrame(
        [duration_parser(t) for t in time_spent], index=["Room 1", "Room 2", "Room 3"], columns=["time"], dtype=float
    )
    start_frame = [summary[1], summary[3], summary[5]]
    start_frame = [int(f) for f in start_frame]

    # Live data for each of the room is separated with the frame slicing number parsed from above.
    df_room_1 = df.iloc[0 : start_frame[1], :]
    df_room_2 = df.iloc[start_frame[1] : start_frame[2], :]
    df_room_3 = df.iloc[start_frame[2] : -1, :]

    # The coordinates of the user_position, camera_attention_point, and controller_attention_point are extracted here,
    # and being fed into a separate dataframe for easier Plotly Express plotting integration.
    coords_room_1 = coords_concat(df_room_1)
    coords_room_2 = coords_concat(df_room_2)
    coords_room_3 = coords_concat(df_room_3)

    # * Plot 1: 2D scatter plots of the top-down views of the game, with user/cam/controller coordinates scattered.
    room_1_scatter = scatter_figure(
        coords_room_1,
        title="Room 1",
        img="https://cdn.jsdelivr.net/gh/spencerwooo/situational-awareness-vr/DataAnalysis/room_topdown/room1.png",
        img_x=-20.5,
        img_y=15.2,
        img_sizex=35,
        img_sizey=37.6,
    )
    room_2_scatter = scatter_figure(
        coords_room_2,
        title="Room 2",
        img="https://cdn.jsdelivr.net/gh/spencerwooo/situational-awareness-vr/DataAnalysis/room_topdown/room2.png",
        img_x=35.6,
        img_y=15.2,
        img_sizex=35.6,
        img_sizey=37.6,
    )
    room_3_scatter = scatter_figure(
        coords_room_3,
        title="Room 3",
        img="https://cdn.jsdelivr.net/gh/spencerwooo/situational-awareness-vr/DataAnalysis/room_topdown/room3.png",
        img_x=88.5,
        img_y=14.7,
        img_sizex=35,
        img_sizey=37.6,
    )

    # * Plot 2: 3D interactive coordinates plotted directly as scatter plots.
    room_1_3d_fig = interactive_3d_figure(coords_room_1, "Room 1")
    room_2_3d_fig = interactive_3d_figure(coords_room_2, "Room 2")
    room_3_3d_fig = interactive_3d_figure(coords_room_3, "Room 3")

    # * Plot 3: Benchmark of the player's time spent clearing each room.
    time_benchmark = px.bar(time_spent, y=time_spent.index, x="time", color=time_spent.index)

    # * Plot 4: Histogram of the cam/controller attention object's distance-to-player.
    cam_dist_hist = px.histogram(
        df, x="camera_hit_dist", marginal="rug", nbins=50, title="Histogram of camera attention distance to player"
    )
    con_dist_hist = px.histogram(
        df,
        x="controller_hit_distance",
        marginal="rug",
        nbins=50,
        title="Histogram of controller attention distance to player",
    )

    # * Plot 5: Bar plot of the number of unique game objects brought to attention during gameplay.
    cam_attention_count = aggregate_attention_obj(df, column_name="camera_hit_obj")
    cam_attention = px.bar(
        cam_attention_count,
        x="camera_hit_obj",
        y="count",
        color="camera_hit_obj",
        title="Camera attention game objects",
    )
    con_attention_count = aggregate_attention_obj(df, column_name="controller_hit_obj")
    con_attention = px.bar(
        con_attention_count,
        x="controller_hit_obj",
        y="count",
        color="controller_hit_obj",
        title="Controller attention game objects",
    )

    return html.Div(
        [
            html.Hr(),
            html.Div(
                [
                    html.Div("Player Time Spent Clearing Each Room", className="font-bold"),
                    html.Div(
                        [
                            html.P(
                                "Summary of your performance for the game is visualised here. The time you spent "
                                "observing and gathering information inside the virtual environment is a decisive "
                                "factor in your decision making, and will impact your attention of your surroundings. ",
                                className="text-gray-400 w-1/3",
                            ),
                            dcc.Graph(id="time_benchmark", figure=time_benchmark, className="h-72"),
                        ],
                        className="flex justify-between",
                    ),
                ]
            ),
            html.Hr(),
            html.Div(
                [
                    html.Div("Top-down Coordinate Visualisation", className="font-bold"),
                    html.P(
                        "Three of the gameplay coordinates are visualised as scatter plots with the z-index omitted: "
                        "user_position, camera_hit_point, and controller_hit_point. These coordinates indicate the "
                        "points you been through and exact coordinates that you paid attention to or interacted with.",
                        className="text-gray-400",
                    ),
                ],
            ),
            html.Div(
                [
                    dcc.Graph(id="scatter_vis", figure=room_1_scatter),
                    dcc.Graph(id="scatter_vis", figure=room_2_scatter),
                    dcc.Graph(id="scatter_vis", figure=room_3_scatter),
                ],
                className="grid grid-cols-3",
            ),
            html.Hr(),
            html.Div(
                [
                    html.Div("Interactive 3D Scatter Visualisation", className="font-bold"),
                    html.P(
                        "The coordinates are in a 3D virtual world. Hence, here we are visualising them in 3D. The 3D "
                        "scatter plots are interactive, where you can dive directly into the plots and investigate the "
                        "actual coordinations that best describe situational awareness.",
                        className="text-gray-400",
                    ),
                ]
            ),
            html.Div(
                [
                    dcc.Graph(id="scatter_3d_vis", figure=room_1_3d_fig),
                    dcc.Graph(id="scatter_3d_vis", figure=room_2_3d_fig),
                    dcc.Graph(id="scatter_3d_vis", figure=room_3_3d_fig),
                ],
                className="grid grid-cols-3",
            ),
            html.Hr(),
            html.Div("Game objects that were attended to or interacted with", className="font-bold"),
            html.Div(
                [
                    dcc.Graph(id="camera_attention", figure=cam_attention),
                    dcc.Graph(id="controller_attention", figure=con_attention),
                ],
                className="grid grid-cols-2",
            ),
            html.Div("Histogram of attention game objects' distance to player", className="font-bold"),
            html.Div(
                [
                    dcc.Graph(id="camera_hit_dist_histo", figure=cam_dist_hist),
                    dcc.Graph(id="controller_hit_dist_histo", figure=con_dist_hist),
                ],
                className="grid grid-cols-2",
            ),
            html.Hr(),
            html.Div("Raw data processed and analysed:", className="font-sm text-gray-400 font-bold"),
            html.Div(f"{csv_filename} - {datetime.fromtimestamp(csv_date)}", className="font-sm text-gray-400"),
            dash_table.DataTable(
                data=df.head(100).to_dict("records"), columns=[{"name": i, "id": i} for i in df.columns], page_size=6
            ),
            html.Div(f"{txt_filename} - {datetime.fromtimestamp(txt_date)}", className="font-sm text-gray-400"),
            html.Div(
                txt_content,
                className="block whitespace-pre overflow-x-scroll font-mono text-xs border bg-gray-100 p-2 rounded",
            ),
        ],
        className="flex flex-col space-y-4",
    )


@app.callback(
    Output("output-visualise", "children"),
    Input("upload-csv", "contents"),
    State("upload-csv", "filename"),
    State("upload-csv", "last_modified"),
)
def update_output(content: list[str, str], name: list[str, str], date: list[str, str]) -> html.Div:
    """Dash application callback - when a file is uploaded, then the relevant output is updated accordingly if both of
    the uploaded files are of the right format and can be parsed normally.

    Args:
        content (list[str, str]): Contents of the uploaded file - base64 encoded.
        name (list[str, str]): The filenames of both of the uploaded files.
        date (list[str, str]): The last modified date of the two files.

    Returns:
        html.Div: Dash HTML component - warning message, or rendered visualisation component.

    """
    if content is not None:
        if len(content) == 2:
            return parse_uploaded_file(content, name, date)
        else:
            return html.Div(
                "CSV and TXT files should be uploaded together.",
                className="flex flex-1 flex-col items-center justify-center font-bold text-red-400",
            )
    else:
        return html.Div(
            [
                html.P("Waiting for upload ...", className="font-bold"),
                html.P("Please upload the CSV and the TXT files together.", className="text-gray-300"),
            ],
            className="flex flex-1 flex-col items-center justify-center",
        )


if __name__ == "__main__":
    app.run_server(debug=False)
