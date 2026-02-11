# Procedural Maze Demo (Unity)

https://www.youtube.com/watch?v=ThcsYr0NK4E

This project is a procedural maze system built in Unity using a custom implementation of Wave Function Collapse (WFC) combined with multiple manually implemented pseudo-random number generators (PRNGs) and custom rendering effects.

The maze is generated dynamically using constraint-based tile selection. Each tile is represented as a bitmask of possible states, and adjacency rules are enforced through directional compatibility masks. Tiles collapse based on entropy and seeded randomness, ensuring consistent but varied layouts.

The project does not rely solely on Unity’s built-in random system. Instead, it includes custom implementations of:

A Mersenne Twister variant

Linear Congruential Generators (LCG)

XORShift64*

These are used to control collapse decisions and allow deterministic maze generation through seed control.

The maze supports dynamic updating as the player moves. Tile bounds shift, and dependent systems adjust accordingly.

In addition to generation logic, the project includes:

A direction-based message spawning system that places and rotates objects according to the resolved tile type.

A procedural mesh generator capable of creating rounded, beveled cuboid geometry at runtime.

A custom post-processing pipeline using OnRenderImage, with exposure, sharpness, white balance, and edge control parameters.

A custom anti-aliasing shader for smoothing high-contrast maze edges.

A decal shader for overlaying surface details onto tiles.
