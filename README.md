[![Version](https://img.shields.io/badge/version-1.0.0-brightgreen)](https://github.com/miguel93041/accessmysqlconverter)
[![License](https://img.shields.io/badge/license-CCBYNCSA4.0-blue.svg?style=flat)](https://github.com/miguel93041/MS-Translator/blob/master/LICENSE.txt)

- [MS-Translator](#ms-translator)
  * [¿Qué es?](#qué-es)
  * [Instalación](#instalación)
  * [¿Cómo se usa?](#cómo-se-usa)
  * [Uso extendido](#uso-extendido)
    + [Modificar tamaño de bits instrucción, primer operando, segundo operando](#modificar-tamaño-de-bits-instrucción-primer-operando-segundo-operando)
    + [Añadir nuevas instrucciones](#añadir-nuevas-instrucciones)
    + [Crear instrucción inmediata](#crear-instrucción-inmediata)
  * [Contribución](#contribución)
  * [Licencia](#licencia)
  
# MS-Translator
## ¿Qué es?
Si has intentado pasar un programa de ensamblador (.asm) instrucción por instrucción a hexadecimal para posteriormente cargarlo en una memoria (RAM, ROM) de Logisim, 
sabrás el tiempo que consume⏳, más aún si tienes que arreglar alguna línea del programa original y volver a repetir todo el proceso😴... MS-Translator viene a solucionar este problema.

MS-Translator es una aplicación creada para ayudar a transcribir automáticamente archivos .asm a un .txt que contiene todo el programa de ensamblador traducido a hexadecimal para cargar en una memoria (RAM, ROM) de Logisim.

## Instalación
Instalar el programa es muy fácil, simplemente ve a la categoría de **Releases** de GitHub y descargate la última versión. Aunque **te agradecería que te lo descargaras por Wuolah** para que **aportes un granito de arena en mi café☕ de época de exámenes**, gracias❤️.

## ¿Cómo se usa?
¡Usarlo es extremadamente sencillo, simplemente arrastra tu archivo .asm sobre él .exe y el programa empezará la conversión!
![Application Image](https://gyazo.com/7cd1f738ab488ce8fd4e012b4c1eada3.gif)

Por ejemplo este sería un archivo de .asm válido (puede haber comentarios, el programa los omite)
```
.data 63
vector:	 .ascii16 "HELLO WORLD."
cero:    .dw 0
const:   .dw 128
letra:   .rw 1

.data 127
tty_out: .rw 1

.code
		mov cero, tty_out
		mov vector, letra
DOWHILE:mov letra, tty_out
		add const, INST
INST: 	mov vector, letra
		cmp 74, letra
		beq PUNTO
		cmp cero, cero
		beq DOWHILE
PUNTO:	mov letra, tty_out
HALT:	cmp cero, cero
		beq HALT
.end
```

Y este sería el archivo .txt que genera
```
v2.0 raw
a5ff 9fcd a6ff 2604 9fcd 654d c009 65cb
c002 a6ff 65cb c00a 51*0 48 45 4c
4c 4f 20 57 4f 52 4c 44
2e 0 80 0 49*0 0 
```

Y si cargamos él .txt en Logisim

![Logisim Image](https://gyazo.com/05c305b59f40371f023f7100bb11678b.gif)

## Uso extendido
Modificando la configuración del programa es posible **añadir nuevas instrucciones, instrucciones inmediatas y modificar los bits de la instrucción, primer operando y segundo operando**.

El programa genera un .json de configuración la primera vez que se ejecuta.
![Config file](https://gyazo.com/eb4933703145da41a6c38252e2db8cc7.png)

### Modificar tamaño de bits instrucción, primer operando, segundo operando
Para modificar el tamaño deberemos abrir la configuración y cambiar los siguientes parámetros, por defectos configurados así:

![Bit size](https://gyazo.com/3f234194a0c74d3cff24740a3a3a8750.png)

Una vez cambiados guardar el archivo de configuración y lanzar el programa.

*Para algún despistado esto hace referencia a lo siguiente:

![Packet](https://gyazo.com/8ef2fb5c913e01cb8bfe7e0f1e257d6a.png)

### Añadir nuevas instrucciones
Para añadir/eliminar instrucciones deberemos abrir la configuración y añadir/borrar nuevos elementos en el parámetro *instructions*, las instrucciones por defecto son add (0), cmp (1), mov (2) y beq (3)

![Add instruction](https://gyazo.com/1ca9db4c1d6e872de912b5ba016e0ae4.png)

*Name* hace referencia al nombre de la instrucción

*Value* hace referencia al valor en decimal de la instrucción

*Immediate* hace referencia a si la instrucción es inmediata o no (si desconoces que es esto, déjalo en false, en el punto de abajo lo explico)

### Crear instrucción inmediata
Si tenemos una instrucción que solo requiere de un operando (el segundo) y queremos usar los bits vacíos del primer operando para seleccionar una operación extra junto con los bits de la instrucción deberemos marcar el parámetro *immediate* como true y en *value* deberemos juntar los bits de instrucción más los del primer operando (empezando por el bit de mayor peso), es decir...
![Immediate](https://gyazo.com/2a85ab90efaff440a683792c39d13e3e.png)

## Contribución
Si queréis contribuir para mejorar el proyecto siempre podéis hacerme un **Pull Request** o si, por el contrario, encontráis algún error podéis crear uno en **Issues**.

## Licencia
[LICENCIA](https://github.com/miguel93041/MS-Translator/blob/master/LICENSE.txt) que utiliza este programa.
