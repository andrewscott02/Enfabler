using BehaviourTrees;

public class BasicBehaviourTree : BehaviourTree
{
    protected override Node SetupTree()
    {
        Node root = new Roam(agent, 30f, 1f, 6f);

        return root;
    }
}
