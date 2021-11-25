#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import pandas as pd


def duration_parser(delta: str) -> str:
    """Parse time duration string into a float-like string.

    Args:
        delta (str): The time duration formatted as '00:01:45.292135'

    Returns:
        str: The time duration as seconds formatted as float-like string

    """
    t, f = delta.split(".")
    h, m, s = t.split(":")
    return f"{int(h) * 60 * 60 + int(m) * 60 + int(s)}.{f}"


def coords_transform(df: pd.DataFrame) -> pd.DataFrame:
    columns = [df.columns[c] for c in [0, 2, 1]]
    df = df.astype(float)
    df = df.reindex(columns=columns)
    df = df.rename(columns={columns[0]: "x", columns[1]: "y", columns[2]: "z"})
    df["z"] = df["z"].apply(lambda x: -x)
    df["type"] = columns[0].rsplit("_", 1)[0]
    return df


def split_coords_series(series: pd.Series) -> pd.DataFrame:
    name = series.name

    return coords_transform(
        series.apply(lambda x: x[1:-1])
        .str.split(",", expand=True)
        .rename(columns={0: f"{name}_x", 1: f"{name}_y", 2: f"{name}_z"})
    )


def coords_concat(df: pd.DataFrame) -> pd.DataFrame:
    user_pos = split_coords_series(df["user_position"])
    cam_hit = split_coords_series(df["camera_hit_point"])
    con_hit = split_coords_series(df["controller_hit_point"])
    return user_pos.append([cam_hit, con_hit])
