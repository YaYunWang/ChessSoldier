import random
import re
from KBEDebug import *
from array import *


class FBBattleField(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)

	def initBattle(self, player_cell):
		self.addTimer(10, 0, 100)

	def onTimer(self, id, userArg):
		if(userArg == 100):
			self.delTimer(id)
			self.base.FinishBattleField()