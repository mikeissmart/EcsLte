using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using EcsLte;
using System.Linq;
using static UnityEditor.Progress;
using System;
using System.Collections;
using EcsLte.Unity.Debugging.Scripts.Behaviours;

namespace EcsLte.Unity.Debugging.Editor.Windows
{
    public class EntityWindow : EditorWindow
    {
        private EcsContext _selectedContext;
        private Entity[] _cacheEntities = new Entity[0];
        private List<Entity> _entities = new List<Entity>();
        private Entity _selectedEntity;
        private EntityFilter _entityFilter;
        private List<ComponentConfig> _availableConfigs = new List<ComponentConfig>();
        private List<ComponentConfig> _whereAllOfListConfigs = new List<ComponentConfig>();
        private List<ComponentConfig> _whereAnyOfListConfigs = new List<ComponentConfig>();
        private List<ComponentConfig> _whereNoneOfListConfigs = new List<ComponentConfig>();

        private PopupField<EcsContext> _contextDropDown;
        private PopupField<ComponentConfig> _whereAllOfDropDown;
        private ListView _whereAllOfList;
        private PopupField<ComponentConfig> _whereAnyOfDropDown;
        private ListView _whereAnyOfList;
        private PopupField<ComponentConfig> _whereNoneOfDropDown;
        private ListView _whereNoneOfList;
        private ListView _entityLayout;
        private bool _guiCreated;

        private EntityInspectorBehaviour _entityInspector;

        [MenuItem("Tools/EcsLte/EntityWindow")]
        public static void ShowExample()
        {
            EntityWindow wnd = GetWindow<EntityWindow>();
            wnd.titleContent = new GUIContent("EntityWindow");
        }

        private void OnEnable()
        {
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
                    RefreshEntities();
                }
            }
            else
            {
                ClearLayout();
                _entityInspector = null;
            }
        }

        private void CreateLayout()
        {
            _guiCreated = true;

            rootVisualElement.Add(CreateContextLayout());

            var title = new Label("Entities");
            title.style.alignSelf = Align.Center;
            rootVisualElement.Add(title);
            var splitView = new TwoPaneSplitView(0, 100, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);

            splitView.Add(CreateEntityLayout());
            splitView.Add(CreateEntityFilterLayout());
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

        private VisualElement CreateEntityLayout()
        {
            var visualEle = new VisualElement();

            _entityLayout = new ListView(
                _entities,
                15,
                () => new Label(),
                (item, index) => { (item as Label).text = _entities[index].ToString(); });
            _entityLayout.selectionType = SelectionType.Single;
            _entityLayout.style.flexGrow = 1f;
            _entityLayout.onSelectionChange += items => OnEntitySelectChange((Entity)items.First());
            visualEle.Add(_entityLayout);

            return visualEle;
        }

        private VisualElement CreateEntityFilterLayout()
        {
            var visualEle = new VisualElement();

            visualEle.Add(new Label("Entity Filter"));

            _availableConfigs = ComponentConfigs.Instance.AllComponentConfigs.ToList();

            visualEle.Add(CreateComponentConfigList("Where All Of",
                _availableConfigs, _whereAllOfListConfigs,
                OnWhereAllOfAdd, OnWhereAllOfRemove,
                out _whereAllOfDropDown, out _whereAllOfList));

            visualEle.Add(CreateComponentConfigList("Where Any Of",
                _availableConfigs, _whereAnyOfListConfigs,
                OnWhereAnyOfAdd, OnWhereAnyOfRemove,
                 out _whereAnyOfDropDown, out _whereAnyOfList));

            visualEle.Add(CreateComponentConfigList("Where None Of",
                _availableConfigs, _whereNoneOfListConfigs,
                OnWhereNoneOfAdd, OnWhereNoneOfRemove,
                 out _whereNoneOfDropDown, out _whereNoneOfList));

            return visualEle;
        }

        private VisualElement CreateComponentConfigList(string text,
            List<ComponentConfig> popupConfigs, List<ComponentConfig> listConfigs,
            Action addAction, Action<ComponentConfig> removeAction,
            out PopupField<ComponentConfig> configPopup, out ListView configList)
        {
            var visualEle = new VisualElement();
            visualEle.style.flexGrow = 1;

            var row = CreateRow();
            configPopup = new PopupField<ComponentConfig>(
                text,
                popupConfigs,
                0,
                x => x.ComponentType.Name,
                x => x.ComponentType.Name);
            row.Add(configPopup);

            var addButton = new Button(addAction);
            addButton.text = "+";
            row.Add(addButton);
            visualEle.Add(row);

            configList = new ListView(
                listConfigs,
                15,
                () =>
                {
                    var subRow = CreateRow();
                    subRow.style.flexGrow = 1f;
                    subRow.style.flexShrink = 0f;
                    subRow.style.flexBasis = 0f;
                    subRow.Add(new Button() { text = "X", style = { color = Color.red } });
                    subRow.Add(new Label());

                    return subRow;
                },
                (item, index) =>
                {
                    (item.ElementAt(1) as Label).text = listConfigs[index].ComponentType.ToString();
                    if (_availableConfigs.Contains(listConfigs[index]))
                        listConfigs.RemoveAt(listConfigs.LastIndexOf(listConfigs[index]));
                    else
                        (item.ElementAt(0) as Button).clicked += () => removeAction(listConfigs[index]);
                });
            configList.style.flexGrow = 1f;
            var scrollable = new ScrollView(ScrollViewMode.Vertical) { style = { flexGrow = 1 } };
            scrollable.contentContainer.style.flexGrow = 1;

            scrollable.Add(configList);
            visualEle.Add(scrollable);

            return visualEle;
        }

        private void ClearLayout()
        {
            _guiCreated = false;
            _selectedContext = null;
            Array.Clear(_cacheEntities, 0, _cacheEntities.Length);
            _entities.Clear();

            rootVisualElement.Clear();
        }

        private void ContextChanged(EcsContext context)
        {
            _selectedContext = context;
            _entityFilter = context.Filters.CreateFilter();
        }

        private void RefreshEntities()
        {
            if (!_selectedContext.Entities.HasEntity(_selectedEntity))
            {
                _selectedEntity = Entity.Null;
            }
            var count = _selectedContext.Entities.GetEntities(_entityFilter, ref _cacheEntities);
            _entities.Clear();
            _entities.AddRange(_cacheEntities.Take(count));

            _entityLayout.Refresh();
        }

        private void OnEntitySelectChange(Entity entity)
        {
            var inspector = GetEntityInspector();
            inspector.Entity = entity;
            inspector.Context = _selectedContext;
        }

        private void OnWhereAllOfAdd()
        {
            if (_availableConfigs.Remove(_whereAllOfDropDown.value))
            {
                _whereAllOfListConfigs.Add(_whereAllOfDropDown.value);
                _whereAllOfListConfigs.Sort();
                _entityFilter.WhereAllOf(_whereAllOfDropDown.value);

                _whereAllOfDropDown.choices = _availableConfigs;
                _whereAllOfList.Refresh();
            }
        }

        private void OnWhereAnyOfAdd()
        {
            if (_availableConfigs.Remove(_whereAnyOfDropDown.value))
            {
                _whereAnyOfListConfigs.Add(_whereAnyOfDropDown.value);
                _whereAnyOfListConfigs.Sort();
                _entityFilter.WhereAnyOf(_whereAnyOfDropDown.value);

                _whereAnyOfList.Refresh();
            }
        }

        private void OnWhereNoneOfAdd()
        {
            if (_availableConfigs.Remove(_whereNoneOfDropDown.value))
            {
                _whereNoneOfListConfigs.Add(_whereNoneOfDropDown.value);
                _whereNoneOfListConfigs.Sort();
                _entityFilter.WhereNoneOf(_whereNoneOfDropDown.value);

                _whereNoneOfList.Refresh();
            }
        }

        private void OnWhereAllOfRemove(ComponentConfig config)
        {
            _whereAllOfListConfigs.Remove(config);
            _availableConfigs.Add(config);
            _availableConfigs.Sort();
            RecreateEntityFilter();


            _whereAllOfDropDown.choices = _availableConfigs;
            _whereAllOfList.Refresh();
        }

        private void OnWhereAnyOfRemove(ComponentConfig config)
        {
            _whereAnyOfListConfigs.Remove(config);
            _availableConfigs.Add(config);
            _availableConfigs.Sort();
            RecreateEntityFilter();
        }

        private void OnWhereNoneOfRemove(ComponentConfig config)
        {
            _whereNoneOfListConfigs.Remove(config);
            _availableConfigs.Add(config);
            _availableConfigs.Sort();
            RecreateEntityFilter();
        }

        private void RecreateEntityFilter()
        {
            _entityFilter = _selectedContext.Filters.CreateFilter();

            foreach (var config in _whereAllOfListConfigs)
                _entityFilter.WhereAllOf(config);
            foreach (var config in _whereAnyOfListConfigs)
                _entityFilter.WhereAnyOf(config);
            foreach (var config in _whereNoneOfListConfigs)
                _entityFilter.WhereNoneOf(config);
        }

        private EntityInspectorBehaviour GetEntityInspector()
        {
            if (_entityInspector == null)
            {
                _entityInspector = new GameObject("EcsLte-EntityDebugger")
                    .AddComponent<EntityInspectorBehaviour>();
                _entityInspector.gameObject.isStatic = true;
            }

            return _entityInspector;
        }
    }
}