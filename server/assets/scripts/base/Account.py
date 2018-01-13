# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class Account(KBEngine.Proxy):
	def __init__(self):
		KBEngine.Proxy.__init__(self)

		KBEngine.globalData["Halls"].ReqAddPlayer(self)
		
	def onTimer(self, id, userArg):
		"""
		KBEngine method.
		使用addTimer后， 当时间到达则该接口被调用
		@param id		: addTimer 的返回值ID
		@param userArg	: addTimer 最后一个参数所给入的数据
		"""
		DEBUG_MSG(id, userArg)
		
	def onEntitiesEnabled(self):
		"""
		KBEngine method.
		该entity被正式激活为可使用， 此时entity已经建立了client对应实体， 可以在此创建它的
		cell部分。
		"""
		INFO_MSG("account[%i] entities enable. mailbox:%s" % (self.id, self.client))
			
	def onLogOnAttempt(self, ip, port, password):
		"""
		KBEngine method.
		客户端登陆失败时会回调到这里
		"""
		INFO_MSG(ip, port, password)
		return KBEngine.LOG_ON_ACCEPT
		
	def onClientDeath(self):
		"""
		KBEngine method.
		客户端对应实体已经销毁
		"""
		DEBUG_MSG("Account[%i].onClientDeath:" % self.id)
		self.destroy()

	def ReCreateAccountRequest(self, role_type, name):
		self.RoleName = name
		self.RoleType = role_type

		self.client.ReCreateAccountResponse(0)

	def QueryPlayerCountRequest(self):
		KBEngine.globalData["Halls"].QueryPlayerCount(self)

	def GetPlayerCount(self, player_count):
		self.client.QueryPlayerCountResponse(player_count)

	def EntryFBSceneRequest(self, scene_id):
		KBEngine.globalData["Halls"].EntryFBScene(self)

	def OnClientMsg_March(self, message):
		if self.client == None:
			return

		self.client.onMarchMsg(message)

	def OnEnterFB(self, fb):
		self.currentBattleField = fb
		self.currentBattleField.AccountReady()

	def creatAvatar(self, battleField):
		prarm = {
			'battlefiled':battleField,
			'roleName':self.RoleName,
			'roleType':self.RoleType,
			'account':self					
			}

		self.Avatar = KBEngine.createBaseLocally("Avatar", prarm)
		# 这里需要客户端接受到消息后，切换场景，并且切换场景完成后，调用服务器的reqHasEnteredBattlefiled 方法。
		self.client.onInitBattleField()

	def reqHasEnteredBattlefiled(self):
		self.giveClientTo(self.Avatar)
		self.Avatar.onClientInit()