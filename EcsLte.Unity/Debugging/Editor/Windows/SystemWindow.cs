using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace EcsLte.Unity.Debugging.Editor.Windows
{
    public class SystemWindow : EditorWindow
    {
        private EcsContext _selectedContext;
        private SystemBase[] _cacheSystems = new SystemBase[0];
        private List<SystemBase> _systems = new List<SystemBase>();
        private SystemBase _selectedSystem;

        private PopupField<EcsContext> _contextDropDown;
        private ListView _systemLayout;
        private VisualElement _selectedSystemLayout;
        private bool _guiCreated;

        [MenuItem("Tools/EcsLte/SystemWindow")]
        public static void ShowExample()
        {
            SystemWindow wnd = GetWindow<SystemWindow>();
            wnd.titleContent = new GUIContent("SystemWindow");
        }

        private void Update()
        {
            if (EditorApplication.isPlaying)
            {
                if (!_guiCreated)
                    CreateLayout();
                else if (!EditorApplication.isPaused)
                {
                    if (_selectedContext != _contextDropDown.value)
                        ContextChanged(_contextDropDown.value);
                    RefreshSystems();
                }
            }
            else
            {
                ClearLayout();
            }
        }

        private void CreateLayout()
        {
            _guiCreated = true;

            rootVisualElement.Add(CreateContextLayout());

            var title = new Label("Systems");
            title.style.alignSelf = Align.Center;
            rootVisualElement.Add(title);
            var splitView = new TwoPaneSplitView(0, 100, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);

            splitView.Add(CreateSystemLayout());
            splitView.Add(CreateSelectedSystemLayout());
        }

        private VisualElement CreateContextLayout()
        {
            var row = CreateRow();

            _contextDropDown = new PopupField<EcsContext>(
                "Contexts",
                EcsContexts.Instance.GetAllContexts().ToList(),
                0,
                x => x?.Name ?? "",
                x => x?.Name ?? "");
            row.Add(_contextDropDown);

            return row;
        }

        private VisualElement CreateRow()
            => new VisualElement { style = { flexDirection = FlexDirection.Row } };

        private VisualElement CreateColumn()
            => new VisualElement { style = { flexDirection = FlexDirection.Column } };

        private VisualElement CreateSystemLayout()
        {
            var visualEle = new VisualElement();
            visualEle.style.flexGrow = 1;

            _systemLayout = new ListView(
                _systems,
                20,
                () =>
                {
                    var rootItem = CreateRow();

                    var titleLabel = new Label();
                    titleLabel.style.alignContent = Align.FlexStart;
                    rootItem.Add(titleLabel);

                    var timeLabel = new Label();
                    timeLabel.style.alignContent = Align.FlexEnd;
                    rootItem.Add(timeLabel);

                    return rootItem;
                },
                (item, index) =>
                {
                    var system = _systems[index];

                    var title = item.Children().ElementAt(0) as Label;
                    title.text = system.GetType().Name;
                    title.style.flexGrow = 1f;
                    title.style.color = system.IsActive
                        ? Color.black
                        : Color.gray;

                    var time = item.Children().ElementAt(1) as Label;
                    time.text = system.UpdateMilliseconds.ToString();
                });
            _systemLayout.selectionType = SelectionType.Single;
            _systemLayout.style.flexGrow = 1f;
            _systemLayout.onSelectionChange += items => UpdateSelectedSystemLayout((SystemBase)items.First());
            visualEle.Add(_systemLayout);

            return visualEle;
        }

        private VisualElement CreateSelectedSystemLayout()
        {
            var visualEle = new VisualElement();

            var titleRow = CreateRow();
            titleRow.Add(new Button());
            titleRow.Add(new Label());
            visualEle.Add(titleRow);

            var timeRow = CreateRow();
            timeRow.style.height = 30;
            timeRow.style.maxHeight = 30;
            timeRow.style.flexGrow = 1f;
            visualEle.Add(timeRow);
            for (var i = 0; i < 4; i++)
            {
                var subCol = CreateColumn();
                subCol.style.flexGrow = 1f;
                timeRow.Add(subCol);
                subCol.Add(new Label());
                subCol.Add(new Label());
            }
            visualEle.style.display = DisplayStyle.None;
            visualEle.style.flexGrow = 1f;
            _selectedSystemLayout = visualEle;

            return visualEle;
        }

        private void UpdateSelectedSystemLayout(SystemBase system)
        {
            _selectedSystem = system;

            if (_selectedSystem == null)
            {
                _selectedSystemLayout.style.display = DisplayStyle.None;
                return;
            }
            else
                _selectedSystemLayout.style.display = DisplayStyle.Flex;

            var titleRow = _selectedSystemLayout.Children().ElementAt(0);
            var actBtn = titleRow.ElementAt(0) as Button;
            var title = titleRow.ElementAt(1) as Label;

            title.text = _selectedSystem.GetType().Name;
            if (_selectedSystem.IsActive)
            {
                actBtn.text = "Deactivate";
                actBtn.style.color = Color.red;
                actBtn.clicked += () => DeactivateSystem(_selectedSystem);
            }
            else
            {
                actBtn.text = "Activate";
                actBtn.style.color = Color.green;
                actBtn.clicked += () => ActivateSystem(_selectedSystem);
            }

            var timeRow = _selectedSystemLayout.Children().ElementAt(1);
            (timeRow.Children().ElementAt(0).Children().ElementAt(0) as Label).text = "Initialize";
            (timeRow.Children().ElementAt(0).Children().ElementAt(1) as Label).text = system.InitializeMilliseconds.ToString();

            (timeRow.Children().ElementAt(1).Children().ElementAt(0) as Label).text = "Activated";
            (timeRow.Children().ElementAt(1).Children().ElementAt(1) as Label).text = system.ActivatedMilliseconds.ToString();

            (timeRow.Children().ElementAt(2).Children().ElementAt(0) as Label).text = "Deactivated";
            (timeRow.Children().ElementAt(2).Children().ElementAt(1) as Label).text = system.DeactivatedMilliseconds.ToString();

            (timeRow.Children().ElementAt(3).Children().ElementAt(0) as Label).text = "Uninitialize";
            (timeRow.Children().ElementAt(3).Children().ElementAt(1) as Label).text = system.UninitializeMilliseconds.ToString();
        }

        private void ClearLayout()
        {
            _guiCreated = false;
            _selectedContext = null;
            Array.Clear(_cacheSystems, 0, _cacheSystems.Length);
            _systems.Clear();

            rootVisualElement.Clear();
        }

        private void ContextChanged(EcsContext context)
        {
            _selectedContext = context;
        }

        private void RefreshSystems()
        {
            var count = _selectedContext.Systems.GetAllSystems(ref _cacheSystems);
            _systems.Clear();
            _systems.AddRange(_cacheSystems.Take(count));

            _systemLayout.Refresh();
            UpdateSelectedSystemLayout(_selectedSystem);
        }

        private void ActivateSystem(SystemBase system)
        {
            _selectedContext.Systems.ActivateSystem(system);
        }

        private void DeactivateSystem(SystemBase system)
        {
            _selectedContext.Systems.DeactivateSystem(system);
        }
    }
}
