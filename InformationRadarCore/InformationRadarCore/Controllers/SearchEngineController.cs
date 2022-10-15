namespace InformationRadarCore.Controllers

public class SearchEngineController
{
    SearchEngine searchEngine;
    Lighthouse lighthouse;

    public SearchEngineController ( Lighthouse lighthouse )
    {
        this.searchEngine = new SearchEngine ();
        this.lighthouse = lighthouse;
    }

    public void RunSearch ()
    {
        this.searchEngine.RunSearch ( this.lighthouse );
    }
}
