import base64
import io
from datetime import datetime

import dash
import pandas as pd
import plotly.express as px
from dash import dash_table, dcc, html
from dash.dependencies import Input, Output, State

from utils.plot_3d import coords_concat

external_stylesheets = ["https://unpkg.com/tailwindcss@^2/dist/tailwind.min.css"]

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
        html.Div(id="output-visualise", className="container mx-auto p-5 flex flex-1"),
        html.Footer(
            "Situational Awareness Visualisation and Analysis ©️2021 - Shangbo Wu",
            className="p-5 text-sm text-gray-400 bg-gray-50 text-center",
        ),
    ],
    className="h-screen flex flex-col",
)


def parse_uploaded_file(contents, filename, date):
    _, content_string_0 = contents[0].split(",")
    _, content_string_1 = contents[1].split(",")
    decoded_0 = base64.b64decode(content_string_0)
    decoded_1 = base64.b64decode(content_string_1)

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

    def duration_parser(delta):
        t, f = delta.split(".")
        h, m, s = t.split(":")
        return f"{int(h) * 60 * 60 + int(m) * 60 + int(s)}.{f}"

    summary = [line.split(":", 1)[-1] for line in txt_content.splitlines()]
    time_spent = [summary[0], summary[2], summary[4]]
    time_spent = pd.DataFrame(
        [duration_parser(t) for t in time_spent], index=["Room 1", "Room 2", "Room 3"], columns=["time"], dtype=float
    )
    start_frame = [summary[1], summary[3], summary[5]]
    start_frame = [int(f) for f in start_frame]

    df_room_1 = df.iloc[0 : start_frame[1], :]
    df_room_2 = df.iloc[start_frame[1] : start_frame[2], :]
    df_room_3 = df.iloc[start_frame[2] : -1, :]

    coords_room_1 = coords_concat(df_room_1)
    coords_room_2 = coords_concat(df_room_2)
    coords_room_3 = coords_concat(df_room_3)

    def interactive_3d_figure(coords, title):
        fig = px.scatter_3d(
            coords, x="x", y="y", z="z", color="type", symbol="type", opacity=0.6, width=540, height=540, title=title
        )
        fig.update_layout(
            legend=dict(yanchor="bottom", y=1.02, xanchor="left", x=0.01, itemsizing="constant"),
            legend_title_text=None,
        )
        fig.update_traces(marker={"size": 3})
        return fig

    room_1_3d_fig = interactive_3d_figure(coords_room_1, "Room 1")
    room_2_3d_fig = interactive_3d_figure(coords_room_2, "Room 2")
    room_3_3d_fig = interactive_3d_figure(coords_room_3, "Room 3")

    time_benchmark = px.bar(time_spent, y=time_spent.index, x="time", color=time_spent.index)

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

    def aggregate_attention_obj(column_name):
        return (
            df.groupby(column_name)
            .count()
            .reset_index()
            .rename(columns={"frame_no": "count"})
            .sort_values(["count"], ascending=False)
        )

    cam_attention_count = aggregate_attention_obj(column_name="camera_hit_obj")
    cam_attention = px.bar(
        cam_attention_count,
        x="camera_hit_obj",
        y="count",
        color="camera_hit_obj",
        title="Camera attention game objects",
    )
    con_attention_count = aggregate_attention_obj(column_name="controller_hit_obj")
    con_attention = px.bar(
        con_attention_count,
        x="controller_hit_obj",
        y="count",
        color="controller_hit_obj",
        title="Controller attention game objects",
    )

    return html.Div(
        [
            html.Div(f"{csv_filename} - {datetime.fromtimestamp(csv_date)}"),
            dash_table.DataTable(
                data=df.head(100).to_dict("records"), columns=[{"name": i, "id": i} for i in df.columns], page_size=6
            ),
            html.Div(f"{txt_filename} - {datetime.fromtimestamp(txt_date)}"),
            html.Div(
                txt_content,
                className="block whitespace-pre overflow-x-scroll font-mono text-xs border bg-gray-100 p-2 rounded",
            ),
            html.Hr(),
            dcc.Graph(id="time_benchmark", figure=time_benchmark),
            html.Hr(),
            html.Div("Top-down Coordinate Visualisation"),
            html.Hr(),
            html.Div("Interactive 3D Scatter Visualisation"),
            html.Div(
                [
                    dcc.Graph(id="scatter_3d_vis", figure=room_1_3d_fig),
                    dcc.Graph(id="scatter_3d_vis", figure=room_2_3d_fig),
                    dcc.Graph(id="scatter_3d_vis", figure=room_3_3d_fig),
                ],
                className="grid grid-cols-3",
            ),
            html.Hr(),
            html.Div(
                [
                    dcc.Graph(id="camera_attention", figure=cam_attention),
                    dcc.Graph(id="controller_attention", figure=con_attention),
                ],
                className="grid grid-cols-2",
            ),
            html.Div(
                [
                    dcc.Graph(id="camera_hit_dist_histo", figure=cam_dist_hist),
                    dcc.Graph(id="controller_hit_dist_histo", figure=con_dist_hist),
                ],
                className="grid grid-cols-2",
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
def update_output(content, name, date):
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
    app.run_server(debug=True)
