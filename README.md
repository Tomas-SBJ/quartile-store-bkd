# Desafio Técnico - QuartileStore

Solução completa para o desafio técnico, implementando uma API REST para gerenciamento de Lojas/Empresas e um microserviço Serverless para Produtos.

## Arquitetura
* **Linguagem:** C# com .NET 9
* **Padrões de Projeto:** Padrão de Repositório e Unit of Work
* **Banco de Dados:** SQL Server com EF Core
* **APIs:**
    1.  **API de Lojas:** ASP.NET Core Web API
    2.  **Microserviço de Produtos:** Azure Functions (Isolated Worker Model)
* **CI/CD:** GitHub Actions com deploy para Staging e Swap manual para Produção.
* **Testes:**
    * **Testes de Unidade:** xUnit e Moq para a camada de Serviço.
    * **Testes de Integração:** Postman com testes automatizados.

## Endpoints no Azure
* **API de Lojas (Swagger):** `https://quartilestore-dmedacane7dhenfh.westus2-01.azurewebsites.net/swagger`
* **Microserviço de Produtos (Base URL):** `https://quartilestore-functions-gmeeakbmh3abaxfe.westus2-01.azurewebsites.net/api`
    * *Nota: Os endpoints das Functions requerem uma Function Key (`x-functions-key`) no header. Na collection postman já está inserido o header com a function key.*

## Regras de Negócio Implementadas

Esta seção destaca as principais regras de negócio e restrições de integridade que foram implementadas na arquitetura da solução, garantindo a robustez e a consistência dos dados.

### Company (Empresa)

* **Identificador de Negócio:** O `Code` de uma empresa é um `int` e deve ser **único** em todo o sistema.
* **Criação (`POST /api/companies`):**
    * Retorna `201 Created` em caso de sucesso, com o objeto da empresa criada.
    * Retorna `409 Conflict` se uma empresa com o mesmo `Code` já existir.
    * Retorna `400 Bad Request` se os dados enviados forem inválidos (ex: campos obrigatórios faltando).
* **Exclusão (`DELETE /api/companies/{companyCode}`):**
    * Uma empresa **não pode** ser excluída se ela possuir uma ou mais lojas associadas (Regra `Restrict`).
    * A tentativa de exclusão de uma empresa com lojas vinculadas resultará em uma resposta `409 Conflict`, protegendo a integridade dos dados.

### Store (Loja)

* **Hierarquia:** Uma `Store` deve obrigatoriamente pertencer a uma `Company` existente.
* **Identificador de Negócio:** O `Code` de uma `Store` é um `int` e deve ser único **dentro do escopo de sua empresa-mãe**.
    * *Exemplo:* Duas empresas diferentes (`Company A` e `Company B`) podem ter uma loja com o mesmo `Code` (ex: `1`). No entanto, a `Company A` não pode ter duas lojas com `Code = 1`.
* **Criação (`POST /api/companies/{companyCode}/stores`):**
    * Retorna `201 Created` em caso de sucesso.
    * Retorna `404 Not Found` se a `companyCode` informada na URL não existir.
    * Retorna `409 Conflict` se uma loja com o mesmo `Code` já existir para aquela empresa.
  * Retorna `400 Bad Request` se os dados enviados forem inválidos (ex: campos obrigatórios faltando).
* **Exclusão (`DELETE .../stores/{storeCode}`):**
    * Uma loja **não pode** ser excluída se ela possuir um ou mais produtos associados (Regra `Restrict`).
    * A tentativa de exclusão de uma loja com produtos vinculados resultará em uma resposta `409 Conflict`, protegendo a integridade dos dados

### Product (Produto)

* **Hierarquia:** Um `Product` deve obrigatoriamente pertencer a uma `Store` existente.
* **Identificador de Negócio:** O `Code` de um `Product` é um `int` e deve ser único **dentro do escopo de sua loja-mãe**.
* **Criação (`POST .../stores/{storeCode}/products`):**
    * Retorna `201 Created` em caso de sucesso.
    * Retorna `404 Not Found` se a hierarquia de `companyCode` / `storeCode` na URL for inválida.
    * Retorna `409 Conflict` se um produto com o mesmo `Code` já existir naquela loja.
    * Retorna `400 Bad Request` se os dados enviados forem inválidos (ex: campos obrigatórios faltando).

### Convenções Gerais da API

* **Respostas de Erro:** Todas as respostas de erro (`4xx`, `5xx`) seguem um formato JSON padronizado (`ApiErrorResponse`), contendo `statusCode`, `title`, `detail`. Propriedades nulas são omitidas do JSON de resposta.
* **Validação:** Erros de validação de dados em `POST` ou `PUT` retornam `400 Bad Request` com um objeto `errors` que detalha os campos e as mensagens de erro.
* **Nomenclatura:** A API expõe seus dados em JSON usando o padrão `camelCase` (ex: `companyCode`), enquanto no banco de dados a nomenclatura segue o padrão `snake_case` (ex: `company_code`), com a conversão sendo gerenciada automaticamente pelo EF Core.
* **Requisições GET:**
    * Endpoints que buscam um recurso individual (ex: `GET /api/companies/{id}`) retornam `404 Not Found` se o recurso não existir.
    * Endpoints que buscam uma coleção (ex: `GET /api/companies/{id}/stores`) retornam `200 OK` com uma lista vazia (`[]`) se não houver itens.
---