import os

def CreateLink(link, target):
	pardir = link[:link.rfind("\\")]
	if not os.path.isdir(pardir):
		os.makedirs(pardir)

	os.system("mklink /J " + link + " " + target)


#-------#

if os.path.isdir("LinkSrc"):
	os.system("rmdir /s /q LinkSrc")

CreateLink("LinkSrc\\ProjectK\\Base", "..\\ProjectK\\Assets\\Scripts\\ProjectK\\Base")
CreateLink("LinkSrc\\ProjectK\\Settings", "..\\ProjectK\\Assets\\Scripts\\ProjectK\\Settings")
CreateLink("LinkSrc\\EditorK\\Shared", "..\\ProjectK\\Assets\\Scripts\\EditorK\\Shared")
