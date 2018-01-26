# -*- coding: utf-8 -*-
import KBEngine
import random
import time
from KBEDebug import *

class Hall(KBEngine.Base):
	def __init__(self):
		DEBUG_MSG("Hall init")

		self.RoomCount = 10

		KBEngine.Base.__init__(self)
		KBEngine.globalData["Halls"] = self

		self.rooms = []

		addTimer(0.1, 0.1, 1)

	def onTimer(self, id, userArg):
		if(userArg == 1):
			roomNum = len(self.rooms)
			if(roomNum >= self.RoomCount):
				delTimer(id)
			else:
				createRoom(roomNum)

	def createRoom(self, index):
		params = {
			"index":index
		}

		room = KBEngine.createBaseLocally("Room", params)

	def appendRoom(self, room):
		self.rooms.append(room, [])

	def ReRoomInfoRequest(self, account):
		pass