using UnityEngine;
using UnityEngine.InputSystem;

public class ProcedureTester : MonoBehaviour
{
    [SerializeField] ProcedureManager manager;

    void Update()
    {
        if (manager == null) return;
        if (Keyboard.current == null) return;

        // Utilisation de la propriété .wasPressedThisFrame du nouveau Input System
        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            TestStep1();
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            manager.ReportAction("SWITCH_OFF");
            Debug.Log("Action SWITCH_OFF validée !");
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            manager.ReportAction("CLOSE_VALVE");
            Debug.Log("Action CLOSE_VALVE validée !");
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.numpad4Key.wasPressedThisFrame)
        {
            manager.ReportAction("PLACE_LOCK");
            Debug.Log("Action PLACE_LOCK validée !");
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame || Keyboard.current.numpad5Key.wasPressedThisFrame)
        {
            manager.ReportAction("PLACE_TAG");
            Debug.Log("Action PLACE_TAG validée !");
        }
    }

    [ContextMenu("Tester Etape 1 (PRESS_STOP)")]
    public void TestStep1()
    {
        if (manager != null)
        {
            manager.ReportAction("PRESS_STOP");
            Debug.Log("Action PRESS_STOP validée manuellement !");
        }
    }
}