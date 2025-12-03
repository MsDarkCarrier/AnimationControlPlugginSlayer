using Godot;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

[Tool]
public partial class AnimationControl : Panel
{
    [ExportGroup("GeneralAllAnimations")]
    [Export] public float maxTimeAnimation = 3f;
    [Export] public TypesAnimationEvent typeAnimationControl;
    [Export] public bool activateAnimation = false, hiddeAnimation = false, pausable = true, visibleInScene = false, inverseOpacity;

    private float count = 0f;
    private bool detectorOldPosition = false;
    private CancellationTokenSource animationTokenSource;
    private float objetiveOpacity;

    public bool realTimeActiveAnimation = false;

    public enum TypesAnimationEvent
    {
        Move = 1,
        MoveAndOpacityControl = 2,
        SplashOpacity = 3,
        OpacityControl = 4,
        SplashCycle = 5,
    }

    [ExportGroup("AnimationsMove")]
    [Export] public Vector2 target = new Vector2();
    private Vector2 oldTarget = new Vector2();
    [Export] public MovDirection direction;

    [ExportSubgroup("TargetObject")]
    [Export] public Control objectTarget = null;

    public enum MovDirection
    {
        left = -160,
        right = 160,
        top = 161,
        bottom = -161,
        personaliced = 162,
        none = 163
    };

    Func<float, float, float> PorcentajeCalc = (x, y) => x / y;
    Func<float, float, float, float> DirectionCalc = (actual, target, vel) => Mathf.Lerp(actual, target, vel);

    public override void _Ready()
    {
        NormalicedAnimation();
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint()) return;

        if (activateAnimation && Visible)
        {
            visibleInScene = !visibleInScene;

            // Reiniciar cualquier animación en curso antes de iniciar la nueva
            animationTokenSource?.Cancel();
            animationTokenSource = new CancellationTokenSource();

            switch (typeAnimationControl)
            {
                case TypesAnimationEvent.Move:
                    _ = StartCoroutine(ProcessTime((float)delta), hiddeAnimation, false, animationTokenSource.Token);
                    break;
                case TypesAnimationEvent.MoveAndOpacityControl:
                    _ = StartCoroutine(ProcessTimeOpacity((float)delta), hiddeAnimation, false, animationTokenSource.Token);
                    break;
                case TypesAnimationEvent.SplashOpacity:
                    _ = StartCoroutine(ProcessOpacity((float)delta), hiddeAnimation, false, animationTokenSource.Token);
                    break;
                case TypesAnimationEvent.OpacityControl:
                    _ = StartCoroutine(ProcessOpacity((float)delta), hiddeAnimation, false, animationTokenSource.Token);
                    break;
                case TypesAnimationEvent.SplashCycle:
                    _ = StartCoroutine(ProcessOpacity((float)delta), hiddeAnimation, true, animationTokenSource.Token);
                    break;
            }

            activateAnimation = false; // resetea el trigger del botón
        }
    }

    private async Task StartCoroutine(IEnumerable routine, bool disableNode, bool cycleAnimation, CancellationToken token)
    {
        realTimeActiveAnimation = true;
        count = 0f;

        await Task.Delay(50, token).ContinueWith(_ => { }); // pequeña espera opcional

        try
        {
            foreach (IEnumerator frameAnimation in routine)
            {
                if (token.IsCancellationRequested) break;
                while (GetTree().Paused && pausable) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            }
        }
        catch (OperationCanceledException)
        {
            // Cancelación intencional: reiniciar animación
        }
        finally
        {

            realTimeActiveAnimation = false;
            Visible = disableNode ? false : true;
            count = 0f;

            if (typeAnimationControl == TypesAnimationEvent.Move || typeAnimationControl == TypesAnimationEvent.MoveAndOpacityControl)
            {
                detectorOldPosition = !detectorOldPosition;
                (target, oldTarget) = (oldTarget, target);
            }

            objetiveOpacity = (objetiveOpacity == 1) ? 0 : 1;
            // Si la animación era de ciclo (parpadeo), volver a activar
            activateAnimation = (cycleAnimation && !token.IsCancellationRequested) ? true : false;
        }
    }

    // Métodos de proceso
    public IEnumerable ProcessTime(float delta)
    {
        while (count <= maxTimeAnimation)
        {
            count += delta;
            float vel = PorcentajeCalc(count, Math.Max(maxTimeAnimation, 0.0001f));

            float x = DirectionCalc(GlobalPosition.X, target.X, vel);
            float y = DirectionCalc(GlobalPosition.Y, target.Y, vel);
            GlobalPosition = new Vector2(x, y);

            float distance = GlobalPosition.DistanceTo(target);
            if (count >= maxTimeAnimation || distance <= 0.999f)
            {
                count = Mathf.Clamp(count, 0, maxTimeAnimation);
                GlobalPosition = target;
                break;
            }
            yield return null;
        }
    }

    public IEnumerable ProcessTimeOpacity(float delta)
    {
        while (count <= maxTimeAnimation)
        {
            count += delta;
            float vel = PorcentajeCalc(count, Math.Max(maxTimeAnimation, 0.0001f));

            float x = DirectionCalc(GlobalPosition.X, target.X, vel);
            float y = DirectionCalc(GlobalPosition.Y, target.Y, vel);
            float opacity = DirectionCalc(Modulate.A, objetiveOpacity, vel * 2.5f);

            GlobalPosition = new Vector2(x, y);
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, opacity);

            float distance = GlobalPosition.DistanceTo(target);
            if (count >= maxTimeAnimation || distance <= 0.999f)
            {
                count = Mathf.Clamp(count, 0, maxTimeAnimation);
                GlobalPosition = target;
                break;
            }
            yield return null;
        }
    }

    public IEnumerable ProcessOpacity(float delta)
    {
        while (count <= maxTimeAnimation)
        {
            count += delta;
            float vel = PorcentajeCalc(count, Math.Max(maxTimeAnimation, 0.0001f));
            float opacity = DirectionCalc(Modulate.A, objetiveOpacity, vel);

            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, opacity);


            float distance = Mathf.Sqrt(Mathf.Pow((objetiveOpacity - opacity), 2));
            if (count >= maxTimeAnimation || distance <= 0.0005162344f)
            {
                count = Mathf.Clamp(count, 0, maxTimeAnimation);
                //Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, inverse);
                break;
            }
            yield return null;
        }
    }

    // utilidades
    public void ResetAnimation()
    {
        if (Modulate.A == 0 || !Visible || Modulate.A < 1f)
        {
            hiddeAnimation = false;
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1);
            Visible = true;
        }
    }

    public void NormalicedAnimation()
    {
        Action eventAnimationMove = null;


        try
        {
            eventAnimationMove = visibleInScene ? () => NormalicedDirection() : () => NormalicedDirection(true);
        }
        catch (Exception) { }


        switch (typeAnimationControl)
        {
            case TypesAnimationEvent.Move:
                Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1);
                eventAnimationMove?.Invoke();
                break;
            case TypesAnimationEvent.MoveAndOpacityControl:

                objetiveOpacity = inverseOpacity ? 0 : 1;
                Modulate = inverseOpacity ? new Color(Modulate.R, Modulate.G, Modulate.B, 1) : new Color(Modulate.R, Modulate.G, Modulate.B, 0);

                eventAnimationMove?.Invoke();
                break;
            case TypesAnimationEvent.SplashOpacity:
                Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1);
                objetiveOpacity = 0;

                break;
            case TypesAnimationEvent.OpacityControl:
            case TypesAnimationEvent.SplashCycle:
                Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
                objetiveOpacity = 1;
                break;
        }
    }

    public void NormalicedDirection(bool inverse = false)
    {
        if (direction == MovDirection.personaliced)
        {
            target = objectTarget.Position;
            oldTarget = GlobalPosition;
            return;
        }


        float objectWidth = Size.X;
        float objectHeight = Size.Y;

        float screenResolutionX = (float)ProjectSettings.GetSetting("display/window/size/viewport_width");
        float screenResolutionY = (float)ProjectSettings.GetSetting("display/window/size/viewport_height");

        Vector2 globalPosi = new Vector2();
        Vector2 targetPosi = new Vector2();


        switch (direction)
        {
            case MovDirection.top:
                globalPosi = new Vector2(GlobalPosition.X, 0);
                targetPosi = new Vector2(GlobalPosition.X, -objectWidth);

                break;
            case MovDirection.bottom:
                globalPosi = new Vector2(GlobalPosition.X, screenResolutionY - objectWidth);
                targetPosi = new Vector2(GlobalPosition.X, screenResolutionY + 20);
                break;
            case MovDirection.left:
                globalPosi = new Vector2(0, GlobalPosition.Y);
                targetPosi = new Vector2(-objectHeight, GlobalPosition.Y);
                break;
            case MovDirection.right:
                globalPosi = new Vector2(screenResolutionX - objectWidth, GlobalPosition.Y);
                targetPosi = new Vector2(screenResolutionX + 20, GlobalPosition.Y);
                break;
        }

        if (inverse)
        {
            GlobalPosition = targetPosi;
            target = globalPosi;
        }
        else
        {
            GlobalPosition = globalPosi;
            target = targetPosi;
        }

        oldTarget = GlobalPosition;
    }
}
