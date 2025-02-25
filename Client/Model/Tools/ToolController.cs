namespace CringeCraft.Client.Model.Tools;

public class ToolController {
    
    private readonly Dictionary<string, ITool> _tools = []; 
    private ITool _currentTool;
    private const string _defaultKey = "Line";

    public ToolController() {
        _currentTool = _tools[_defaultKey];

        _tools.Add("Line", new LineTool());
        _tools.Add("Rectangle", new RectangleTool());
        _tools.Add("Ellips", new EllipsTool());
        _tools.Add("Polygon", new PolygonTool());
        _tools.Add("Star", new StarTool());
    }

    public void ChangeTool(string toolName) {
        _currentTool = _tools[toolName];
    }

}