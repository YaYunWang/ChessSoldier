# -*- coding: utf-8 -*-
import KBEngine
import random
import time
from KBEDebug import *

class FBBattleField(KBEngine.Base):
	
	def __init__(self):
		KBEngine.Base.__init__(self)

		self.MsgToClient_March("第一步：验证玩家是否在线...")

		self.player.OnEnterFB(self)

	def MsgToClient_March(self, msg):
		self.player.OnClientMsg_March(msg)

	def AccountReady(self):
		self.MsgToClient_March("第二步：创建战场cell...")
		self.createInNewSpace(None)

	def onGetCell(self):
		DEBUG_MSG('cell has been created')
		self.MsgToClient_March("第三步：创建玩家Avatar...")
		self.player.creatAvatar(self)

	def AvatarRegiste(self,avatarCellMailBox):
		self.MsgToClient_March("第四步：初始化战场...")
		self.player_cell = avatarCellMailBox
		self.cell.initBattle(self.player_cell)

	def FinishBattleField(self):
		pass
