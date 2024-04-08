# Descripción-Test

Esta es la prueba técnica para desarrollador Back-end .NET de Dasigno SAS.

Esta **API REST** fue desarrollada en **.NET 8** desde **Visual Studio 2022**, la cual se conecta a una base de datos **SQL Server**.

Para iniciar el servicio web que se encuentra en la carpeta **/back-dasigno-test**, únicamente se debe configurar la cadena de conexión a la base de datos **"SQLConnection"** en el archivo de configuración **appsettings.json** ubicado en la raíz del proyecto. Luego, solo debe abrirse y ejecutarse el proyecto desde Visual Studio, se creará la base de datos automáticamente si no lo está, y se hará la correspondiente migración de las tablas de cada entidad, luego se mostrará mediante la interfaz de **SWAGGER** todos los endpoints correspondientes para todas las operaciones (CRUD).

Para este servicio web se denominó la entidad **"Employee"** o **"Empleado"**, para realizar todas las operaciones (CRUD) correspondientes para la prueba. También se agregó una entidad aparte llamada **"ErrorLog"** que únicamente registrará los logs de error en la base de datos. Esta entidad no cuenta con ningún endpoint para operaciones CRUD.

Este servicio cuenta con un modelo de respuesta llamado **ResultModel** que devolverá la siguiente información en formato .JSON, a los usuarios tras consumir cualquiera de los endpoints:

```json
{
  "data": [...],
  "error": false,
  "message": "Success",
  "state": 200,
  "page": 1,
  "itemsPerPage": 100,
  "totalItems": 100,
  "totalPages": 1
}
```

## Filtros

Para filtrar por ID, primer nombre **(firstname)** o primer apellido **(surname)**, desde la interfaz de **SWAGGER** o donde se desee consumir, se debe asignar el valor por el que se quiere filtrar en el parámetro **"filter"**, y nombre del atributo a filtrar en el parámetro **"filterType"**, es decir, si yo deseo filtrar por primer apellido, aquellos empleados que tengan el apellido **"Torres"**, debo dar el valor **"Torres"** al parámetro **"filter"**, y debo dar el valor **"surname"** al parámetro **"filterType"**. Se deben agregar a la petición ambos valores válidos (no nulos), en caso contrario el servicio no tendrá en cuenta ningún filtro y traerá los registros de manera predeterminada.

## Paginación

Para realizar la paginación, se debe agregar a la petición los parámetros que contengan el número de la página que se desea traer, y la cantidad de registros por página. Se deben agregar a la petición ambos valores válidos (no nulos), en caso contrario el servicio no tendrá en cuenta la paginación y traerá todos los registros que se hayan solicitado.

## Pruebas automáticas Postman

En la carpeta **/PostmanAutomaticTest** se encuentra una colección de Postman donde se encuentra la configuración de una petición de tipo **POST** **/setEmployee** para realizar la carga automática de registros válidos, **(Datos del archivo "TestDataNoError.json")**, y registros con datos no válidos o nulos **(Datos del archivo "TestDataNull.json")**.

