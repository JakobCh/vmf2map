# vmf2map
Quick and dirty Source .vmf to Valve format .map converter.

Made using python 3.5.4 but it's not exactly using anything special so I think it should work on 2.x

Obviously you wont be getting any displacements, if there are displacements in the .vmf file they'll show up as normal brushes.

Primaryly made this to convert Source maps into quake maps since the newer quake bsp compilers support the valve format.

# Usage:
python vmf2map.py <in_map.vmf> <out_map.map>

# Example:
python vmf2map.py .\koth_sawmill_d.vmf .\koth_sawmill.map
