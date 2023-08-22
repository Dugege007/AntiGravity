using UnityEngine;

public class EjectorStats : MonoBehaviour
{
    public EjectorData_SO templateData;
    public EjectorData_SO ejectorData;
    public BuffData_SO buffData;

    private void OnEnable()
    {
        if (templateData != null)
            ejectorData = Instantiate(templateData);
    }

    public int MaxHealth
    {
        get { if (ejectorData != null) return ejectorData.maxHealth; else return 0; }
        set { ejectorData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { if (ejectorData != null) return ejectorData.currentHealth; else return 0; }
        set { ejectorData.currentHealth = Mathf.Clamp(value, 0, MaxHealth); }
    }

    public float BaseSelfAngleSpeed
    {
        get { if (ejectorData != null) return ejectorData.baseSelfAngleSpeed; else return 0; }
        set { ejectorData.baseSelfAngleSpeed = value; }
    }

    public float MaxSpeed
    {
        get { if (ejectorData != null) return ejectorData.maxSpeed; else return 0; }
        set { ejectorData.maxSpeed = value; }
    }

    public float BaseArcLengthSpeed
    {
        get { if (ejectorData != null) return ejectorData.baseArcLengthSpeed; else return 0; }
        set { ejectorData.baseArcLengthSpeed = value; }
    }
}
