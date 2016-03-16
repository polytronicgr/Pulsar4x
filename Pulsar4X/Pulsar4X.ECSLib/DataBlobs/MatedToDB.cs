using System;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// DataBlob to define what this Entity is mated to.
    /// If mated, the entity will automatically follow the Root Entity.
    /// </summary>
    public class MatedToDB : TreeHierarchyDB
    {
        public MatedToDB(Entity parent) : base(parent) { }

        public override object Clone()
        {
            return new MatedToDB(Parent);
        }
        
        // TODO: Review access control
        [PublicAPI]
        public static void MateEntities([NotNull] Entity parent, [NotNull] Entity child)
        {
            if (parent == null || !parent.IsValid)
            {
                throw new ArgumentNullException(nameof(parent));
            }
            if (child == null || !child.IsValid)
            {
                throw new ArgumentNullException(nameof(child));
            }

            var parentMatedDB = parent.GetDataBlob<MatedToDB>();

            if (parentMatedDB == null)
            {
                parentMatedDB = new MatedToDB(parent);
                parent.SetDataBlob(parentMatedDB);
            }

            var childMatedDB = child.GetDataBlob<MatedToDB>();
            if (childMatedDB == null)
            {
                childMatedDB = new MatedToDB(parent);
                child.SetDataBlob(childMatedDB);
            }

            childMatedDB.SetParent(parent);
        }

        // TODO: Review access control
        [PublicAPI]
        public static void UnMateEntities([NotNull] Entity parent, [NotNull] Entity child)
        {
            var parentMatedDB = parent?.GetDataBlob<MatedToDB>();
            if (parentMatedDB == null)
            {
                throw new ArgumentException("Parent entity malformed: Does not contain MatedToDB.");
            }

            var childMatedDB = child?.GetDataBlob<MatedToDB>();
            if (childMatedDB == null)
            {
                throw new ArgumentException("Child entity malformed: Does not contain MatedToDB.");
            }

            childMatedDB.SetParent(child);
            parentMatedDB.RemoveChild(child);
        }
    }
}
