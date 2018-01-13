# -*- coding: utf-8 -*-
import KBEngine
import random
import time
from KBEDebug import *

class Hall(KBEngine.Base):
	def __init__(self):
		DEBUG_MSG("Hall init")
		KBEngine.Base.__init__(self)
		KBEngine.globalData["Halls"] = self

		self.players = []

		self.addTimer(1, 5, 1)

	def onTimer(self, id, userArg):
		if userArg == 1:
			self.UpdataPlayer()

	def ReqAddPlayer(self, player_mailbox):
		if player_mailbox in self.players:
			return

		self.players.append(player_mailbox)

	def ReqRemovePlayer(self, player_mailbox):
		if not player_mailbox in self.players:
			return
			
		sefl.players.remove(player_mailbox)

	def UpdataPlayer(self):
		for i in range(len(self.players)):
			if self.players[i].isDestroyed == True:
				del self.players[i]
				self.UpdataPlayer()
				return


	def QueryPlayerCount(self, account):
		player_count = len(self.players)
		account.GetPlayerCount(player_count)

	def EntryFBScene(self, account):
		prarm = {
			"player":account
		}

		BattleField = KBEngine.createBaseAnywhere("FBBattleField", prarm)

