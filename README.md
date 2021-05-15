# Logik
A tool for creating models and calculating stuff, somewhere between a spreadsheet and a programming language

This is a temporary README, proper documentation will (hopefully) follow at some point.

# Motivation
This tool is meant for building small "calculators", for which usually spreadsheets are used.
It attempts to solve some issues with spreadsheets, namely:
* You cannot see formula and value at the same time
* The grid layout can be cumbersome when working with cells which don't need it, as moving them around can sometimes break your references.
* It can be hard to follow the flow if calculations, how each cell affects another

# Building
Logik is implementing using the [Godot Engine](https://godotengine.org/). Why? Because it provides ready made components for GUI, it is fast, and I can program in C#. Also because Logik is not meant to be an online tool.
