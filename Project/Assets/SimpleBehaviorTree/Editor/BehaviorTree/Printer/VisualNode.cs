using System.Collections.Generic;

namespace SimpleBehaviorTree.Editor {
    public class VisualNode {
        private readonly List<VisualNode> _children = new List<VisualNode>();
        private readonly NodePrintController _printer;
        private bool _taskActive;

        public INode Node { get; }
        public IReadOnlyList<VisualNode> Children => _children;
        
        public float Width { get; } = 70;
        public float Height { get; } = 50;
        
        public IGraphBox Box { get; private set; }
        public IGraphBox Divider { get; private set; }
        public float DividerLeftOffset { get; private set; }

        public VisualNode (INode task, IGraphContainer parentContainer) {
            Node = task;
            BindTask();
            
            var container = new GraphContainerVertical();

            AddBox(container);

            if (task.Children != null) {
                var childContainer = new GraphContainerHorizontal();
                foreach (var child in task.Children) {
                    _children.Add(new VisualNode(child, childContainer));
                }
                
                AddDivider(container, childContainer);
                container.AddBox(childContainer);
            }

            parentContainer.AddBox(container);
            
            _printer = new NodePrintController(this);
        }

        private void BindTask () {
            Node.TaskActive.AddListener(UpdateTaskActiveStatus);
        }

        public void RecursiveTaskUnbind () {
            Node.TaskActive.RemoveListener(UpdateTaskActiveStatus);
            
            foreach (var child in _children) {
                child.RecursiveTaskUnbind();
            }
        }

        private void UpdateTaskActiveStatus () {
            _taskActive = true;
        }

        private void AddDivider (IGraphContainer parent, IGraphContainer children) {
            Divider = new GraphBox {
                SkipCentering = true,
            };

            DividerLeftOffset = children.ChildContainers[0].Width / 2;
            var dividerRightOffset = children.ChildContainers[children.ChildContainers.Count - 1].Width / 2;
            var width = children.Width - DividerLeftOffset - dividerRightOffset;

            Divider.SetSize(width, 1);

            parent.AddBox(Divider);
        }

        private void AddBox (IGraphContainer parent) {
            Box = new GraphBox();
            Box.SetSize(Width, Height);
            Box.SetPadding(10, 10);
            parent.AddBox(Box);
        }

        public void Print () {
            _printer.Print(_taskActive);
            _taskActive = false;

            foreach (var child in _children) {
                child.Print();
            }
        }
    }
}