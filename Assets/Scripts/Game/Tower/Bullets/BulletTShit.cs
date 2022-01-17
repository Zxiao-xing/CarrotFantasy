using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTShit : BulletBase
{
    ShitDeBuffProperty shitDeBuffProperty;

    private void Start()
    {
        shitDeBuffProperty = new ShitDeBuffProperty ();
        shitDeBuffProperty.liveTime = 1 + 0.1f * towerLevel;
        shitDeBuffProperty.desSpeed = 1 + towerLevel;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Monster")&&collision.gameObject.activeSelf)
            collision.SendMessage("DesSpeed", shitDeBuffProperty);
        base.OnTriggerEnter2D(collision);
    }

}

public struct ShitDeBuffProperty
{
   public float liveTime;
    public float desSpeed;
}
