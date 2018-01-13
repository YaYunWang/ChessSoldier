# -*- coding: utf-8 -*-
import KBEngine
import random
from KBEDebug import *

class Avatar(KBEngine.Entity):
	
	def __init__(self):
		KBEngine.Entity.__init__(self)
		DEBUG_MSG('Avatar.cell::__init__')