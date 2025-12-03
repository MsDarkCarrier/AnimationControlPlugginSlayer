# AnimationControlPlugginSlayer

Este plugin proporciona un sistema completo para animar elementos UI en Godot Engine utilizando C#.
Es compatible con animaciones de movimiento, opacidad, splash, combinaciones y ciclos, permitiendo crear interfaces dinámicas y responsivas sin necesidad de AnimationPlayer.

✨ Características

*Animaciones transicionales basadas en posición.
*Control de opacidad y efectos de desvanecimiento.
*Animaciones combinadas (movimiento + opacidad).
*Splash animations y ciclos automáticos.
*Soporte para límites de UI y objetos objetivo.
*Totalmente personalizable mediante propiedades exportadas.
*Funciona en tiempo real en el editor gracias a [Tool].


⚙️ Funcionalidades

Ajusta la animación deseada mediante las propiedades exportadas.

Activa la animación cambiando activateAnimation = true.

Puedes combinar comportamiento según eventos o señales.

Compatible con pausa del juego si pausable está activado.


📦 Instalación

Copia la carpeta addons/AnimationControlPlugin/ a tu proyecto.

Activa el plugin desde
Project → Project Settings → Plugins → Animation Control.

Añade un nodo AnimationControl desde el panel de nodos.


📘🛠️ Configuración

En el apartado de GeneralAllAnimation define lo siguiente: 

MaxTimeAnimation = El tiempo de animación, mientras más alto más lento se movera la animación
TypeAnimationControl= Es un enumerador donde podrás elegir las animaciones siendo la siguiente lista: 

Move= Mueve un objeto de tipo Panel Contro de un lugar a otro.
MoveAndOpacity= Mueve al objeto pero controlando su opacidad. 
SplashOpacity= Cambia la opacidad partiendo 100% a 0%.
OpacityControl= Cambia la opacidad partiendo de 0% a 100%.
SplashCycle= Cambia dinámicamente la opacidad entre 0% a 100% y nuevamente de 100% a 0% (Ideal para texto o elementos visuales).

HiddeAnimation= Define si al terminar la animación esta se ocultará (es decir: Cambiará a Visible = False).
ActiveAnimation= Activa la animación cuando está en "TRUE".
Pausable= Define si la animación va a poder ser pausada cuando se usa GetTree().Pausable

( Las animaciones de SplashOpacity, OpacityControl y SplashCycle solamente necesitan las configuraciones generales de arriba .☝️ )

Para las animaciones de move tenemos el enumerador Direction correspondiente a la dirección de donde sadrá la animación de movimiento, siendo la siguiente: 

Left= El objeto partirá desde la izquierda de la pantalla hasta ser visible completamente en la UI.
Right= Left= El objeto partirá desde la Derecha de la pantalla hasta ser visible completamente en la UI.
Top= El objeto partirá desde arriba de la pantalla hasta ser visible completamente en la UI.
Bottom= El objeto partirá desde abajo de la pantalla hasta ser visible completamente en la UI.
Personaliced= El objeto partirá desde la posición que tenga hasta un objetivo ya sea de tipo Vector2 o un nodo de tipo Control tomando su GlobalPosition.

Si quieres usar Personaliced deberás asignar su Target o una posición en tipo Vector2. 

Para asignar la posición sin crear un nodo adicional, puedes utilizar Target.
Para asignar la posición de un nodo de tipo Control a seguir deberás asignarlo desde: 

AnimationMove
       -------> TargetObject


⚖️ Licencia

El uso de este plugin está autorizado para cualquier usuario de la plataforma,
siempre y cuando se otorgue crédito al autor y sus contribuyentes.

Autor: Lan Slayer (Alan Garza)
Lenguaje: C#