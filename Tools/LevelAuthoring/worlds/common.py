"""Shared level building helpers (terrain pieces used by many levels)."""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from level_dsl import *  # noqa: F401,F403  (re-exported for the world modules)
import trykli_sim as sim  # noqa: F401

SPRING_FLUSH = 0.1  # a spring whose center is 0.1 below a surface has its top flush with it


def ground_with_recesses(level, x0, x1, top, recess_centers, recess_width=1.1, depth=0.18, h=1.0):
    """Ground from x0 to x1 with recesses (slots) where springs fit flush with the ground."""
    edges = [x0]
    for c in sorted(recess_centers):
        edges += [c - recess_width / 2.0, c + recess_width / 2.0]
    edges.append(x1)
    for i in range(0, len(edges), 2):
        a, b = edges[i], edges[i + 1]
        if b - a > 0.01:
            level.ground(a, b, top=top, h=h)
    for c in recess_centers:
        level.block(c, top - depth - (h - depth) / 2.0, recess_width, h - depth, surface='Ground')
    return [(c, top - SPRING_FLUSH) for c in recess_centers]


def pit_spikes(level, x0, x1, y=-4.6):
    """Row of spikes at the bottom of a pit."""
    level.spikes((x0 + x1) / 2.0, y, x1 - x0)


def start_slope(level, x=-3.6, y=6.2, length=3.0, angle=-20.0):
    """Short slope under the spawn so Trykli starts rolling to the right (or left with a positive angle)."""
    return level.block(x, y, length, 0.4, rot=angle, surface='Platform')


def slope(level, x0, y0, x1, y1, thickness=0.4, surface='Platform'):
    """Static slope whose TOP surface goes from (x0, y0) to (x1, y1)."""
    import math
    dx, dy = x1 - x0, y1 - y0
    length = math.hypot(dx, dy)
    angle = math.degrees(math.atan2(dy, dx))
    nx, ny = -dy / length, dx / length  # left normal (points up for a left-to-right slope)
    cx = (x0 + x1) / 2.0 - nx * thickness / 2.0
    cy = (y0 + y1) / 2.0 - ny * thickness / 2.0
    return level.block(round(cx, 3), round(cy, 3), round(length, 3), thickness, rot=round(angle, 2), surface=surface)


def funnel(level, x, y, width=1.6, depth=0.8):
    """Small V shaped cup (two short slopes) guiding Trykli to x."""
    slope(level, x - width / 2.0 - 0.6, y + depth, x - 0.35, y, surface='Platform')
    slope(level, x + 0.35, y, x + width / 2.0 + 0.6, y + depth, surface='Platform')
