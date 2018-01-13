# -*- coding: utf-8 -*-
import KBEngine
import random
import time
from KBEDebug import *

class Avatar(KBEngine.Proxy):
	"""
	角色实体
	"""
	def __init__(self):
		KBEngine.Proxy.__init__(self)

		#self.bf = self.cellData['battlefiled']
		

	def onClientInit(self):
		self.createCellEntity(self.cellData['battlefiled'].cell)

	def onClientGetCell(self):
		"""
		KBEngine method.
		客户端已经获得了cell部分实体的相关数据
		"""
		self.bf.AvatarRegiste(self.cell)