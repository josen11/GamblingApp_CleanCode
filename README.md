# Prueba Clean Code - GamblingApp
<div id="top"></div>

<!-- ABOUT THE PROJECT -->
## About The Project
![alt text](https://i.ibb.co/jLMZ8NB/chrome-gja-R4c-M4qi.png)

API REST que permita la creacion y control de ruletas, apuestas y jugadores. Inicialmente es necesario tener crear ruletas y jugadores, cada ruleta puede crease con un profit o en 0, de igual manera es necesario asignar credito a los jugadores de tal manera que puedan generar apuestas. Para crear una apuesta es necesario que la ruleta esta aperturada y que sea una apuesta validad (ya sea un numero del 0-35 o un color: negro o rojo), asimismo que la apuesta no excede los 10K. Cuando se crea la apuesta atuomaticamente se descuenta el monto apostado del credito del jugador y se incremente en el profit de la ruleta. Cuando se cierra la ruleta se genera el numero ganador aletaoriamente, para las apuestas de color se considera rojo (numero par) y negro (numero par). Asimismo se identifica las apuestas ganadoras y se paga el premio de X5 apuesta de numero y X1.8 apuesta de color. Se agrega el monto al credigo de los usuarios ganadores y se descuenta el monto del profit de las ruletas.

**Endpoints** 
* **Actions:** Creacion y control de apuestas <em>[Id, CreationDateTime, BetType, Bet, Handle, UserId, RouletteId, IsWinner]</em>

| HTTP Verb | Endpoints            | Descripcion                                |
|   :----:  |----------------------|--------------------------------------------|
| `GET`     | /api/Actions         | Obtener todos las apuestas                 |
| `POST`    | /api/Actions         | Crear apuesta sin restrincciones ni validaciones       |
| `GET`     | /api/Actions/{id}    | Obtener una apuesta por id              |
| `PUT`     | /api/Actions/{id}    | Actualizar una apuesta por id              |
| `DELETE`  | /api/Actions/{id}    | Eliminar una apuesta por id              |
| `POST`    | /api/Actions/create  | Crear apuesta considerando las restriccione de rouleta aperturada, maximo de apuesta y tipo de apuesta. Asimismo incrementar el profit de la ruleta y descontar del credito de jugador el monto apostado  |

* **Roulettes:** Creacion y control de ruletas <em>[Id, Status, CreationDateTime, OpenDateTime, ClousureDateTime, WinnerNumber, Profit]</em>

| HTTP Verb | Endpoints            | Descripcion                                |
|   :----:  |----------------------|--------------------------------------------|
| `GET`     | /api/Roulettes         | Obtener todos las ruletas                 |
| `POST`    | /api/Roulettes         | Crear ruleta sin restrincciones ni validaciones       |
| `GET`     | /api/Roulettes/{id}    | Obtener una ruleta por id              |
| `PUT`     | /api/Roulettes/{id}    | Actualizar una ruleta por id              |
| `DELETE`  | /api/Roulettes/{id}    | Eliminar una ruleta por id              |
| `POST`    | /api/Roulettes/create  | Crear ruleta considerando el response unicamente del id. <strong> Considerar usar postman para enviar como parametro de HEADER el userId</stong>  |
| `POST`    | /api/Roulettes/open/{id}    | Aperturar ruleta para apuestas.  |
| `POST`    | /api/Roulettes/create/{id}    | Cerrar ruleta, generar nro ganador, identificar apuestas ganadoras, calcular y pagar premios a los ganadores, incrementar el credigo de los jugadores, descontar los premios del profit de la ruleta.  |

* **Users:** Creacion y control de jugadores <em>[UserId, Password, Status, Credit]</em>

| HTTP Verb | Endpoints            | Descripcion                                |
|   :----:  |----------------------|--------------------------------------------|
| `GET`     | /api/Roulettes         | Obtener todos los jugadores                 |
| `POST`    | /api/Roulettes         | Crear jugador sin restrincciones ni validaciones       |
| `GET`     | /api/Roulettes/{id}    | Obtener una jugador por id              |
| `PUT`     | /api/Roulettes/{id}    | Actualizar una jugador por id              |
| `DELETE`  | /api/Roulettes/{id}    | Eliminar una jugador por id              |

<!-- GETTING STARTED -->
## Getting Started

Para correr el aplicativo sera necesario instalar previamente los requisitos

### Prerequisites

* .NET 6.0 https://dotnet.microsoft.com/download/dotnet/6.0
* SQL Server Developer Edition https://dotnet.microsoft.com/download/dotnet/6.0
* Script de BD https://drive.google.com/file/d/1Q8bTQ53pcefvW4eqFFcnvB1sDNgkOr_o/view?usp=sharing
* Postman https://www.postman.com/downloads/
* AWS Toolkit https://aws.amazon.com/es/visualstudio/

### Installation

1. Instalar SQL Server y correr los scripts
2. Clonar el repositorio
   ```sh
   git clone https://github.com/josen11/GamblingApp_CleanCode.git
   ```
3. Instalar .NET 6
4. Crear una cuenta en AWS
5. Crear un usuario en AWS y crearle una policy que le permita acciones sobre el servico Amazon CloudWatch Logs
6. Descargar credenciales y configurar archivo credentials
7. Crear un Log Group
8. Modificar los datos de la cadena de conexion y configuracion de Logs en Cloud Watch en el archivo appsettings.json
9. Correr la aplicacion
   ```sh
   dotnet run
   ```
5. Testear API ya sea con Swagger (disponible al momento de abrir el API) o Postman (Endpoint /api/Roulettes/create)
6. Tester el envio de logs a Amazon CloudWatch

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>

