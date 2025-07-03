using UnityEngine;
using UnityEngine.UI;

public class TimedUIHider : IUpdatable
{
    private float timer;
    private float duration;
    private CanvasGroup canvasToHide;
    private System.Action<TimedUIHider> onFinished; // callback para auto-removerse

    public TimedUIHider(CanvasGroup canvas, float durationSeconds, System.Action<TimedUIHider> onFinish)
    {
        this.canvasToHide = canvas;
        this.duration = durationSeconds;
        this.timer = 0f;
        this.onFinished = onFinish;
    }

    public void Tick(float deltaTime)
    {
        timer += deltaTime;

        if (timer >= duration)
        {
            HideGroup();

            onFinished?.Invoke(this); // notifica que terminó para desregistrarse
        }
    }

    private void HideGroup()
    {
        if (canvasToHide == null) return;

        canvasToHide.alpha = 0f;
        canvasToHide.interactable = false;
        canvasToHide.blocksRaycasts = false;
    }
}
