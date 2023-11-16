import os

class Compiler:
	words = "rmc;lmc;add;sub;loop;endloop;exein;exeout;ric;lic;roc;loc;not;or;and;xor;is;more;setin;setout;exit;break;lshift;rshift;copy;paste;clear".split(";")
	@staticmethod
	def compile(code, kernel_version, array_size):
		result = b""

		def get_bytes(num, zf=2):
			n = hex(num)[2:].zfill(zf)
			n = [n[c*2:c*2+2] for c in range(len(n)//2)]
			n.reverse()
			n = "".join(n)
			return bytes.fromhex(n)

		result+=get_bytes(kernel_version, zf=4)
		result+=get_bytes(array_size, zf=4)

		code = code.replace("\n", "")
		code = code.replace("\t", "")

		commands = code.split(";")[:-1]

		for cmd in commands:
			# match cmd:
			# 	case "rmc":
			# 		result+=b"\x00"
			# 	case "lmc":
			# 		result+=b"\x01"
			# 	case "add":
			# 		result+=b"\x02"
			# 	case "sub":
			# 		result+=b"\x03"
			# 	case "loop":
			# 		result+=b"\x04"
			# 	case "endloop":
			# 		result+=b"\x05"
			# 	case "exein":
			# 		result+=b"\x06"
			# 	case "exeout":
			# 		result+=b"\x07"
			# 	case "ric":
			# 		result+=b"\x08"
			# 	case "lic":
			# 		result+=b"\x09"
			# 	case "roc":
			# 		result+=b"\x0A"
			# 	case "loc":
			# 		result+=b"\x0B"
			# 	case "not":
			# 		result+=b"\x0C"
			# 	case "or":
			# 		result+=b"\x0D"
			# 	case "and":
			# 		result+=b"\x0E"
			# 	case "xor":
			# 		result+=b"\x0F"
			# 	case "is":
			# 		result+=b"\x10"
			# 	case "more":
			# 		result+=b"\x11"
			# 	case "setin":
			# 		result+=b"\x12"
			# 	case "setout":
			# 		result+=b"\x13"
			# 	case "exit":
			# 		result+=b"\x14"
			# 	case "break":
			# 		result+=b"\x15"
			# 	case "lshift":
			# 		result+=b"\x16"
			# 	case "rshift":
			# 		result+=b"\x17"
			# 	case "copy":
			# 		result+=b"\x18"
			# 	case "paste":
			# 		result+=b"\x19"
			# 	case "clear":
			# 		result+=b"\x1A"
			if cmd in Compiler.words:
				result+=get_bytes(Compiler.words.index(cmd))

		return result

class BigCompiler:
	compiler_place = os.path.dirname(os.path.abspath(__file__))

	file_exten_in = ".txt"
	file_exten_out = ".run"
	
	symbols = """\x00☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼ !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~⌂АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмноп░▒▓│┤╡╢╖╕╣║╗╝╜╛┐└┴┬├─┼╞╟╚╔╩╦╠═╬╧╨╤╥╙╘╒╓╫╪┘┌█▄▌▐▀рстуфхцчшщъыьэюяЁёЄєЇїЎў°∙·√№¤■ """

	@staticmethod
	def get_num(n):
		if n[0].upper()=="D":
			n = int(n[1:])
		elif n[0].upper()=="H":
			n = int(n[1:], 16)
		elif n[0].upper()=="O":
			n = int(n[1:], 8)
		elif n[0].upper()=="B":
			n = int(n[1:], 2)
		elif n[0].upper()=="L":
			n = int(BigCompiler.symbols.index(n[1]))
		return n

	@staticmethod
	def to_commands(code):
		code = code.replace("\n", "")
		code = code.replace("\t", "")
		return code.split(";")[:-1]

	@staticmethod
	def to_code(commands):
		return "".join([c+";" for c in commands])

	@staticmethod
	def include(commands, path):
		for c in range(len(commands)):
			if commands[c]!="" and commands[c][0]=="!":
				cmd = commands[c][1:].split(" ")

				if cmd[0]=="include":
					inc_path = os.path.dirname(path)+"\\"+cmd[1].replace("/", "\\")+BigCompiler.file_exten_in
					if cmd[1]=="STDLIB":
						inc_path = BigCompiler.compiler_place+"\\STDLIB\\main"+BigCompiler.file_exten_in
					# elif cmd[1]=="STDLIB_DRIVERS":
					# 	inc_path = BigCompiler.compiler_place+"\\STDLIB\\drivers"+BigCompiler.file_exten_in
					# elif cmd[1]=="STDLIB_KEYS":
					# 	inc_path = BigCompiler.compiler_place+"\\STDLIB\\keys"+BigCompiler.file_exten_in
					# elif cmd[1]=="STDLIB_NOTES":
					# 	inc_path = BigCompiler.compiler_place+"\\STDLIB\\notes"+BigCompiler.file_exten_in
					# elif cmd[1]=="STDLIB_DURATIONS":
					# 	inc_path = BigCompiler.compiler_place+"\\STDLIB\\durations"+BigCompiler.file_exten_in
					# elif cmd[1]=="STDLIB_BASICS":
					# 	inc_path = BigCompiler.compiler_place+"\\STDLIB\\basics"+BigCompiler.file_exten_in

					with open(inc_path, "r", encoding="utf-8") as f:
						commands[c] = f.read()
					commands[c] = BigCompiler.to_code(BigCompiler.include(BigCompiler.to_commands(commands[c]), inc_path))
		return commands

	@staticmethod
	def comment(commands):
		com = []
		for c in range(len(commands)):
			if not "//" in commands[c]:
				com.append(commands[c])
		return com

	@staticmethod
	def paste_templates(commands, templates, ind=0, is_tmpl=False):
		c = ind
		com = ""
		is_ignore = False
		while c<len(commands):
			if commands[c]!="":
				if commands[c][0]=="." and not is_ignore:
					tmpl = BigCompiler.paste_templates(commands, templates, ind=templates[commands[c][1:]], is_tmpl=True)
					com+=tmpl+";"
				elif commands[c]=="!endtmpl" and is_tmpl:
					break

				elif "!tmpl" in commands[c]:
					is_ignore = True
				elif commands[c]=="!endtmpl":
					is_ignore = False

				elif not is_ignore:
					com+=commands[c]+";"
			c+=1

		return com

	@staticmethod
	def compile(path):
		code = ""
		with open(path, "r", encoding="utf-8") as f:
			code = f.read()

		print(code+"\n")

		commands = BigCompiler.to_commands(code)

		kernel_version = 0
		array_size = 1
		root_access = 0

		templates = {}

		commands = BigCompiler.to_commands(BigCompiler.to_code(BigCompiler.include(commands, path)))
		print(BigCompiler.to_code(commands)+"\n")
		print(commands,len(commands), "\n")

		commands = BigCompiler.comment(commands)

		for c in range(len(commands)):
			if commands[c]!="" and commands[c][0]=="!":
				cmd = commands[c][1:].split(" ")

				if cmd[0]=="version":
					kernel_version = min(kernel_version, BigCompiler.get_num(cmd[1]))
					commands[c]=""

				if cmd[0]=="memset":
					array_size = max(array_size, BigCompiler.get_num(cmd[1]))
					commands[c]=""

				if cmd[0]=="isroot":
					root_access = 1
					commands[c]=""

				if cmd[0]=="tmpl":
					templates[cmd[1]]=c+1

				if cmd[0]=="set":
					res = ""
					for i in range(len(cmd)-1):
						for b in bin(BigCompiler.get_num(cmd[1+i]))[2:]:
							res+="lshift;"
							if b=="1":
								res+="add;"
						if i!=len(cmd)-1-1:
							res+="rmc;"
					commands[c] = res

				if cmd[0]=="setl":
					res = ""
					for i in range(len(commands[c][1+len("setl")+1:])):
						for b in bin(BigCompiler.get_num("L"+commands[c][1+len("setl")+1+i]))[2:]:
							res+="lshift;"
							if b=="1":
								res+="add;"
						if i!=len(commands[c][1+len("setl")+1:])-1:
							res+="rmc;"
					commands[c] = res

				if cmd[0]=="rep":
					commands[c] = (cmd[1]+";")*BigCompiler.get_num(cmd[2])

		commands = BigCompiler.to_commands(BigCompiler.paste_templates(commands, templates))

		code = BigCompiler.to_code(commands)

		print(code+"\n")

		return Compiler.compile(code, kernel_version, array_size)

# class VeryBigCompiler(BigCompiler):
# 	@staticmethod
# 	def compile(path):
# 		code = ""
# 		with open(path, "r") as f:
# 			code = f.read()

# 		return super().compile(path)

if __name__=="__main__":
	result = BigCompiler.compile(os.path.abspath("code.txt"))
	with open("result.run", "wb") as f:
		f.write(result)

	res_str = "{"
	for i in result:
		res_str+=hex(int(str(i)))+","
	res_str+="}"

	print(res_str)
	input()

# print([i for i in """ ☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼ !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~⌂АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмноп░▒▓│┤╡╢╖╕╣║╗╝╜╛┐└┴┬├─┼╞╟╚╔╩╦╠═╬╧╨╤╥╙╘╒╓╫╪┘┌█▄▌▐▀рстуфхцчшщъыьэюяЁёЄєЇїЎў°∙·√№¤■ """])

# #=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

# import pygame as pg
# pg.init()

# array = pg.surfarray.array3d(pg.image.load(r"D:\Users\Misha\Desktop\lab (Пользовательское).png"))
# for i in range(array.shape[0]):
# 	for j in range(array.shape[1]):
# 		if array[i][j][0]==48:
# 			print("0", end="")
# 		else:
# 			print("1", end="")
# 	print()




# from random import randrange

# vowels = "AEIOUY"
# consonants = "BCDFGHJKLMNPQRSTVWXZ"

# string = ""
# for i in range(3):
# 	v, c = randrange(len(vowels)), randrange(len(consonants))
# 	string += consonants[c] + vowels[v]
# c = randrange(len(consonants))
# string += consonants[c]
# print(string)