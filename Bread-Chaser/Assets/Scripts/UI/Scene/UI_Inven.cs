using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UIScene
{
    enum GameObjects
    {
        Panel
    }


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject gridPanel = Get<GameObject>((int)GameObjects.Panel);
        foreach (Transform child in gridPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<8; i++)
        {
            //기본 이름값이 null이고 매개변수에 이름값이 없는데 그냥 통과된 이유는 해당기능을 이미 호출함수에서 구현해놔서
            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(parent: gridPanel.transform).gameObject;

            UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>();
            invenItem.SetInfo($"집행검{i}번");
        }
    }
}
