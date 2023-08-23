using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//태어날 때 체력이 최대체력이 되게 하고싶다.
//맞으면 체력을 1 감소하고 싶다.
//체력이 변경되면 UI로 표현하고싶다.
public class SSB_BossHP : MonoBehaviour
{
    bool isChange = false;//체력이 변경되었는지
    //현재체력
    [SerializeField] int hp;
    //최대체력
    public int maxHP = 5;
    //UI
    public Slider sliderHP;

    public int HP //함수인데 변수처럼 쓸 수 있는 property를 만든다
    {
        get { return hp; }//쓸 때 
        set
        {
            if (isChange) return;
            isChange = true;
            hp = value;
            //체력이 변경되면 UI로 표현하고싶다.
            sliderHP.value = hp;
        }//셋팅
    }
    // Start is called before the first frame update
    void Start()
    {
        //태어날 때 체력이 최대체력이 되게 하고싶다.
        sliderHP.maxValue = maxHP;
        HP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (isChange)
        {
            isChange = false;
        }
    }
}
