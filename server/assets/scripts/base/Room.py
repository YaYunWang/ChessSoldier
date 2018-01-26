import KBEngine
from KBEDebug import *

class Room(KBEngine.Base):
	def __init__(self):

		DEBUG_MSG("Hall init")
		
		KBEngine.Base.__init__(self)

		createInNewSpace(None)

	def onGetCell(self):
		KBEngine.globalData["Halls"].appendRoom(self)