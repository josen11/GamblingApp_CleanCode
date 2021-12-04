# Prueba Clean Code - GamblingApp
<div id="top"></div>

[![LinkedIn][linkedin-shield]][linkedin-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">
</div>

<!-- ABOUT THE PROJECT -->
## About The Project
[![Product Name Screen Shot][product-screenshot]

API REST que permita la creacion y control de ruletas y apuestas.

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

### Installation

1. Instalar SQL Server y correr los scripts
2. Clonar el repositorio
   ```sh
   git clone https://github.com/josen11/GamblingApp_CleanCode.git
   ```
3. Instalar .NET 6
4. Correr la aplicacio
   ```dotnet
  dotnet run
   ```
5. Testear API ya sea con Swagger (disponible al momento de abrir el API) o Postman (Endpoint /api/Roulettes/create)

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[linkedin-url]: https://www.linkedin.com/in/jos%C3%A9-enrique-aguirre-chavez-8392734b/
[product-screenshot]:https://drive.google.com/file/d/1JbgFofv-2b0pQK6kd2hZYzV-1-3ABFv2/view?usp=sharing
