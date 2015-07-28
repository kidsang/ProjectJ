import os

def CreateLink(link, target):
	os.system("rmdir " + link)
	os.system("mklink /J " + link + " " + target)

os.system("mkdir LinkSrc")
CreateLink("LinkSrc\\ProjectK", "..\\ProjectK\\Assets\\Scripts\\ProjectK")
CreateLink("LinkSrc\\EditorK", "..\\ProjectK\\Assets\\Scripts\\EditorK")