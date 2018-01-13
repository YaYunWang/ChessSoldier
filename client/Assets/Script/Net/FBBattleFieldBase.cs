/*
	Generated by KBEngine!
	Please do not modify this file!
	Please inherit this module, such as: (class FBBattleField : FBBattleFieldBase)
	tools = kbcmd
*/

namespace KBEngine
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	// defined in */scripts/entity_defs/FBBattleField.def
	public abstract class FBBattleFieldBase : Entity
	{
		public EntityBaseMailbox_FBBattleFieldBase baseMailbox = null;
		public EntityCellMailbox_FBBattleFieldBase cellMailbox = null;



		public override void onGetBase()
		{
			baseMailbox = new EntityBaseMailbox_FBBattleFieldBase();
			baseMailbox.id = id;
			baseMailbox.className = className;
		}

		public override void onGetCell()
		{
			cellMailbox = new EntityCellMailbox_FBBattleFieldBase();
			cellMailbox.id = id;
			cellMailbox.className = className;
		}

		public override void onLoseCell()
		{
			cellMailbox = null;
		}

		public override EntityMailbox getBaseMailbox()
		{
			return baseMailbox;
		}

		public override EntityMailbox getCellMailbox()
		{
			return cellMailbox;
		}

		public override void onRemoteMethodCall(Method method, MemoryStream stream)
		{
			switch(method.methodUtype)
			{
				default:
					break;
			};
		}

		public override void onUpdatePropertys(Property prop, MemoryStream stream)
		{
			switch(prop.properUtype)
			{
				case 40001:
					Vector3 oldval_direction = direction;
					direction = stream.readVector3();

					if(prop.isBase())
					{
						if(inited)
							onDirectionChanged(oldval_direction);
					}
					else
					{
						if(inWorld)
							onDirectionChanged(oldval_direction);
					}

					break;
				case 40000:
					Vector3 oldval_position = position;
					position = stream.readVector3();

					if(prop.isBase())
					{
						if(inited)
							onPositionChanged(oldval_position);
					}
					else
					{
						if(inWorld)
							onPositionChanged(oldval_position);
					}

					break;
				case 40002:
					UInt32 spaceID = stream.readUint32();
					spaceID = 0;
					break;
				default:
					break;
			};
		}

		public override void callPropertysSetMethods()
		{
			ScriptModule sm = EntityDef.moduledefs[className];
			Dictionary<UInt16, Property> pdatas = sm.idpropertys;

			Vector3 oldval_direction = direction;
			Property prop_direction = pdatas[1];
			if(prop_direction.isBase())
			{
				if(inited && !inWorld)
					onDirectionChanged(oldval_direction);
			}
			else
			{
				if(inWorld)
				{
					if(prop_direction.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onDirectionChanged(oldval_direction);
					}
				}
			}

			Vector3 oldval_position = position;
			Property prop_position = pdatas[0];
			if(prop_position.isBase())
			{
				if(inited && !inWorld)
					onPositionChanged(oldval_position);
			}
			else
			{
				if(inWorld)
				{
					if(prop_position.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onPositionChanged(oldval_position);
					}
				}
			}

		}
	}
}