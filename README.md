# Dasigno-Test
Esta la prueba técnica para desarrollador Back-end .NET de Dasigno SAS.

Esta **API REST** fue desarrollada en **.NET 8** desde **Visual Studio 2022**, la cual se conecta a una base de datos **SQLServer**.

Para iniciar el servicio web que se encuentra en la carpeta **/back-dasigno-test**, unicamente se debe configurar la cadena de conexión a la base de datos **"SQLConnection"** en el archivo de configuración **appsettings.json** ubicado en la raiz del proyecto, luego solo debe abrirse y ejecutarse el proyecto desde Visual Studio, y se mostrara mediante la interfaz de **SWAGGER** todos los end point correspondientes para todas las operaciones (CRUD).

## Pruebas automaticas Postman

En la carpeta **/PostmanAutomaticTest** se encuentra una colección de postman donde se encuentra la configuración de una petición de tipo **POST** **/setEmployee** para realizar la carga automatica de registros validos, **(Datos del archivo "TestDataNoError.json")**, y registros con datos no validos o nulos **(Datos del archivo "TestDataNull.json")**.
