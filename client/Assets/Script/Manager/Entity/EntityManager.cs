using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : ManagerTemplate<EntityManager>
{
    private static Dictionary<long, BaseEntity> entityMap = new Dictionary<long, BaseEntity>();
	protected override void InitManager()
	{
	}

	public static BaseEntity CreateEntityFromNetObject(Avatar avatar)
	{
		BaseEntity entity;

		GameObject entityGameObject = new GameObject();

		entity = entityGameObject.AddComponent<PlayerEntity>() as BaseEntity;

		if (entity == null)
			return null;

		entity.InitEntity(avatar);

		return entity;
	}
}
