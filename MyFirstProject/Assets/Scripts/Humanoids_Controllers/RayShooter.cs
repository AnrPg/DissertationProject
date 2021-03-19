using System.Collections;
using UnityEngine;
using System.Text;

public class RayShooter : MonoBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    private GameObject _fireball;
    private Camera _camera;
    [SerializeField] private float attackRange = 10.0f;
    private const int targetIndicatorSize = 50;
    private const string targetIndicatorIcon = "  \u2740 \n\u2740  \u2740\n  \u2740";//" ** \n*  *\n*  *\n ** ";
    public const int microcalibration = 20;
    private GUIStyle targetStyle;

    //public GameObject model;

    void Start()
    {
        _camera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        targetStyle = new GUIStyle(GUIStyle.none);
        targetStyle.normal.textColor = Color.white;

        attackRange = fireballPrefab.GetComponent<Fireball>().GetAttackRange();
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Space key was pressed");
            model.transform.Rotate(90, 0, 0);
        }
        */
        RaycastHit targetedObject = CustomizeTargetAim();

        if (Input.GetMouseButtonDown(0))
        {
            CastFireballSpell(targetedObject.point);
        }
    }

    void OnGUI()
    {
        float posX = _camera.pixelWidth / 2 - targetIndicatorSize / 4;
        float posY = _camera.pixelHeight / 2 - targetIndicatorSize / 4;
                
        GUI.Label(new Rect(posX, posY-microcalibration, targetIndicatorSize, targetIndicatorSize), targetIndicatorIcon, targetStyle);
    }
   
    // Checks if the target at which the user currently aims is a real target or just the environment and change color accordingly
    private RaycastHit CustomizeTargetAim()
    {
        Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 0.3f, out hit, attackRange))
        {
            //GameObject hitObject = hit.transform.gameObject;
            ReactiveTarget target = hit.transform.gameObject.GetComponent<ReactiveTarget>();

            if (target != null)
            {
                targetStyle.normal.textColor = new Color(173.0f / 255, 95.0f / 255, 132.0f / 255);
            }
            else
            {
                targetStyle.normal.textColor = Color.white;
            }
        }
        else
        {
            targetStyle.normal.textColor = Color.white;
        }

        return hit;
    }

    private void CastFireballSpell(Vector3 pos)
    {
        if (_fireball == null)
        {
            GameObject projectileFireball = Instantiate(fireballPrefab) as GameObject;
            projectileFireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
            projectileFireball.transform.rotation = transform.rotation;

            Fireball fireballScript = projectileFireball.GetComponent<Fireball>();
            if (fireballScript != null)
            {
                fireballScript.setShooter(transform.gameObject);
                fireballScript.setTargetPoint(pos);
            }
        }
    }
}
