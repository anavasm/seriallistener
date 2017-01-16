# SerialListener

Servicio para windows que lee los datos recividos por el puerto serie (COM2) y los reenvía a un servidor UDP.

## Dependencias

- .NET Framework

## Configuración predeterminada

La configuración predeterminada del puerto serie es:

- PortName: COM2
- BaudRate: 9600
- Parity: None
- StopBits: One
- DataBits: 8
- Handshake: None

Por defecto los datos de conexión utilizados por el cliente UDP son:

- Ip del servidor: 127.0.0.1
- Puerto: 5557

## Utilización

- Clona el repositorio
- Utiilizar la utilidad de línea de comandos **InstallUtil.exe** para instalar el servicio. [Más info](https://msdn.microsoft.com/es-es/library/aa984379.aspx)