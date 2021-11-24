import base64
import io
from datetime import datetime

import dash

# import numpy as np
import pandas as pd
import plotly.express as px
from dash import dash_table, dcc, html
from dash.dependencies import Input, Output, State

external_stylesheets = ["https://codepen.io/chriddyp/pen/bWLwgP.css"]

app = dash.Dash(__name__, external_stylesheets=external_stylesheets)
app.layout = html.Div(
    [
        dcc.Upload(
            id="upload-csv",
            children=html.Div(["Drag and Drop or ", html.A("Select Files")]),
            style={
                "height": "80px",
                "lineHeight": "80px",
                "borderWidth": "2px",
                "borderStyle": "dashed",
                "borderRadius": "5px",
                "textAlign": "center",
                "margin": "10px",
            },
            multiple=False,
        ),
        html.Div(id="output-visualise"),
    ]
)


def parse_uploaded_file(contents, filename, date):
    content_type, content_string = contents.split(",")
    decoded = base64.b64decode(content_string)

    try:
        if "csv" in filename:
            df = pd.read_csv(io.StringIO(decoded.decode("utf-8")))
    except Exception as e:
        print(e)
        return html.Div(["There was an error parsing the file."])

    cam_dist_hist = px.histogram(df, x="camera_hit_dist", marginal="rug", nbins=50)
    con_dist_hist = px.histogram(df, x="controller_hit_distance", marginal="rug", nbins=50)

    cam_attention_count = (
        df.groupby("camera_hit_obj")
        .count()
        .reset_index()
        .rename(columns={"frame_no": "count"})
        .sort_values(["count"], ascending=False)
    )
    cam_attention = px.bar(cam_attention_count, x="camera_hit_obj", y="count", color="camera_hit_obj")
    con_attention_count = (
        df.groupby("controller_hit_obj")
        .count()
        .reset_index()
        .rename(columns={"frame_no": "count"})
        .sort_values(["count"], ascending=False)
    )
    con_attention = px.bar(con_attention_count, x="controller_hit_obj", y="count", color="controller_hit_obj")

    return html.Div(
        [
            html.Div(f"{filename} - {datetime.fromtimestamp(date)}"),
            html.H6(),
            dash_table.DataTable(
                data=df.head(100).to_dict("records"), columns=[{"name": i, "id": i} for i in df.columns], page_size=10
            ),
            html.Hr(),
            html.Div(
                [
                    dcc.Graph(id="camera_attention", figure=cam_attention, style={"width": "50%"}),
                    dcc.Graph(id="controller_attention", figure=con_attention, style={"width": "50%"}),
                ],
                style={"display": "flex"},
            ),
            html.Div(
                [
                    dcc.Graph(id="camera_hit_dist_histo", figure=cam_dist_hist, style={"width": "50%"}),
                    dcc.Graph(id="controller_hit_dist_histo", figure=con_dist_hist, style={"width": "50%"}),
                ],
                style={"display": "flex"},
            ),
            html.Hr(),
            html.Div("Raw Content"),
            html.Pre(contents[0:200] + "...", style={"whiteSpace": "pre-wrap", "wordBreak": "break-all"}),
        ]
    )


@app.callback(
    Output("output-visualise", "children"),
    Input("upload-csv", "contents"),
    State("upload-csv", "filename"),
    State("upload-csv", "last_modified"),
)
def update_output(content, name, date):
    if content is not None:
        return parse_uploaded_file(content, name, date)


if __name__ == "__main__":
    app.run_server(debug=True)
