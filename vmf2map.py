import sys


if not len(sys.argv) == 3:
	print("Usage: main.py <in_file.vmf> <out_file.map>")
	sys.exit()



TEXTURE_CONVERT = {
	"tools/toolsskip": "clip",
	"tools/toolshint": "hintskip",
	"tools/toolsnodraw": "skip",
	"tools/toolsplayerclip": "clip"
}


#f = open("koth_sawmill_d.vmf", "r")
print(sys.argv[1])
f = open(sys.argv[1], "r")
LINES = f.readlines()
f.close()


class ent:
	def __init__(self, Lines, index):
		self.name = Lines[index].replace("\t", "").replace('\n','')
		#print(self.name)
		self.size = 3 #name + { and }
		index += 2
		self.keys = {}
		self.ents = []

		#get all key value pairs
		while '"' in Lines[index]: 
			splitPoint = Lines[index].find(" ")
			#print(splitPoint)
			#print(Lines[index][0:splitPoint].replace('"','').replace('\t',''), Lines[index][splitPoint:].replace('"','').replace('\n','') )

			key = Lines[index][0:splitPoint].replace('"','').replace('\t','')
			value = Lines[index][splitPoint+1:].replace('"','').replace('\n','')
			
			self.keys[ key ] = value
			index += 1
			self.size += 1
			

		#print(self.keys)

		while not "}" in Lines[index]:
			self.ents.append(ent(Lines, index))
			index += self.ents[-1].size
			self.size += self.ents[-1].size

		#print("done ent")

	def hasDisplacement(self):
		out = False
		for en in self.ents:
			if en.name == "solid":
				for side in en.ents:
					if len(side.ents) > 0:
						out = True

		return out

	def ToMapString(self):
		out = "{\n"
		for key in self.keys:
			out += '"' + key + '" "' + self.keys[key] + '"\n'

		for en in self.ents:
			if en.name == "solid":
				out += "{\n"
				for side in en.ents:
					rotation = "0"
					if "rotation" in side.keys:
						rotation = side.keys["rotation"]

					texture = side.keys["material"]
					if texture in TEXTURE_CONVERT:
						texture = TEXTURE_CONVERT[texture]

					out += side.keys["plane"] + " " + texture + " "
					#out += " -0 -0 -0 1 1\n"
					out += "[ " + side.keys["uaxis"][1:side.keys["uaxis"].find("] ") ] + " ] "
					out += "[ " + side.keys["vaxis"][1:side.keys["vaxis"].find("] ") ] + " ] "
					out += rotation + " " #rotation
					out += side.keys["uaxis"][side.keys["uaxis"].find("] ")+2: ] + " "
					out += side.keys["vaxis"][side.keys["vaxis"].find("] ")+2: ] + "\n"
					
				out += "}\n"

		out += "}\n"

		return out

entities = []
currentName = ""

index = 0
while index < len(LINES):
	le = ent(LINES, index)
	index += le.size
	entities.append( le )
	print(le.name)

#print(entities[-1].name)

f = open(sys.argv[2], "w")

f.write("// Game: Quake\n")
f.write("// Format: Valve\n")

for i in entities:
	t = i.ToMapString()
	#print(t)
	f.write(t)

f.close()