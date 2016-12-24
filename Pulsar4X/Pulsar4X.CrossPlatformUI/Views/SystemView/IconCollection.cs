using System.Collections.Generic;
using Pulsar4X.ECSLib;
using Eto.Drawing;
using System;
using Pulsar4X.ViewModel.SystemView;

namespace Pulsar4X.CrossPlatformUI.Views
{
    internal class IconCollection
    {
        public List<IconBase> Icons { get; } = new List<IconBase>();
        public Dictionary<Guid, EntityIcon> IconDict { get; } = new Dictionary<Guid, EntityIcon> ();
		private SystemMap_DrawableVM _vm;
		private Camera2dv2 _camera;
        public IconCollection(SystemMap_DrawableVM vm)
        {
			_vm = vm;
        }

        public void Init(IEnumerable<Entity> entities, Camera2dv2 camera)
        {
            Icons.Clear();
            IconDict.Clear();
			_camera = camera;
            foreach (var item in entities)
            {
				AddIcon(item);
            }
        }

		private void AddIcon(Entity forEntity)
		{
			if (forEntity.HasDataBlob<OrbitDB>() && forEntity.GetDataBlob<OrbitDB>().Parent != null)
			{
				OrbitRing ring = new OrbitRing(forEntity, _camera);

				Icons.Add(ring);
			}
			if (forEntity.HasDataBlob<NameDB>())
				Icons.Add(new TextIcon(forEntity, _camera));

			EntityIcon entIcon = new EntityIcon(forEntity, _camera);
			Icons.Add(entIcon);
			IconDict.Add(forEntity.Guid, entIcon);
		}

        public void DrawMe(Graphics g)
        {
			foreach (var entityItem in _vm.GetIconableEntites()) 
			{
				if (IconDict.ContainsKey(entityItem.Guid))
				{
					IconDict[entityItem.Guid].DrawMe(g);
				}
				else
				{
					AddIcon(entityItem);
					IconDict[entityItem.Guid].DrawMe(g);
				}
			}
            /*foreach (var item in Icons)
            {
                item.DrawMe(g);
            }
            */
        }
    }

    internal interface IconBase
    {
        //sets the size of the icons
        float Scale { get; set; }

        void DrawMe(Graphics g);
    }

}
