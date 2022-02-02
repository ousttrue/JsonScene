using System.Text.Json.Nodes;
using ImGuiNET;

namespace Editor
{
    public class JsonTree
    {
        string _name;
        JsonObject _root;
        // string _selected;

        public JsonTree(string name, JsonObject root)
        {
            _name = name;
            _root = root;
        }


        void Traverse(JsonNode? node, object key, string parent = "")
        {
            ImGuiTreeNodeFlags flag = default;  // const.ImGuiTreeNodeFlags_.SpanFullWidth
            var value = "";
            switch (node)
            {
                case JsonObject obj:
                    value = obj["name"]?.ToString();
                    break;

                case JsonArray arr:
                    value = $"({(arr.Count)})";
                    break;

                default:
                    flag |= ImGuiTreeNodeFlags.Leaf;
                    flag |= ImGuiTreeNodeFlags.Bullet;
                    // flag |= ImGui.TREE_NODE_NO_TREE_PUSH_ON_OPEN
                    value = $"{node}";
                    break;
            }

            ImGui.TableNextRow();
            // col 0
            ImGui.TableNextColumn();
            var open = ImGui.TreeNodeEx($"{key}", flag);
            ImGui.SetItemAllowOverlap();
            // col 1
            ImGui.TableNextColumn();
            var selected = ImGui.Selectable($"{value}###{parent}/{key}", false, ImGuiSelectableFlags.SpanAllColumns);

            if (node == null)
            {
                return;
            }
            if (open)
            {
                var path = node.GetPath();
                switch (node)
                {
                    case JsonArray arr:
                        for (int i = 0; i < arr.Count; ++i)
                        {
                            Traverse(arr[i], i, path);
                        }
                        break;
                    case JsonObject obj:
                        foreach (var (k, v) in obj)
                        {
                            Traverse(v, k, path);
                        }
                        break;
                }
                ImGui.TreePop();
            }
        }

        public void Show(ref bool p_open)
        {
            if (ImGui.Begin(_name))
            {
                var flags = (
                    ImGuiTableFlags.BordersV
                    | ImGuiTableFlags.BordersOuterH
                    | ImGuiTableFlags.Resizable
                    | ImGuiTableFlags.RowBg
                    | ImGuiTableFlags.NoBordersInBody
                );
                if (ImGui.BeginTable("jsontree_table", 2, flags))
                {
                    // header
                    ImGui.TableSetupColumn("key");
                    ImGui.TableSetupColumn("value");
                    ImGui.TableHeadersRow();

                    // body
                    foreach (var (k, v) in _root)
                    {
                        Traverse(v, k);
                    }

                    ImGui.EndTable();
                }
                ImGui.End();
            }
        }
    }
}
