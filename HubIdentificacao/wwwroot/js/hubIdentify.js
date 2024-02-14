"use strict";


//configurações da conexão
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/identifyClient", { // URL do hub e configurações opcionais
        accessTokenFactory: () => generateToken(), // Token de acesso
        transport: signalR.HttpTransportType.WebSockets, // Especificar o transporte para WebSockets
        headers: { // Adiciona cabeçalhos personalizados
            "x-itau-visual-apikey": generateApikeyId(), // Adicionar apikey como cabeçalho
            "x-itau-visual-correlationID": generateCorrelationId(), // Adicionar CorrelationId como cabeçalho            
        }
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();


//Função Inicio da conexão

async function start() {

    try {
        await connection.start().catch(err => console.error(err.toString()));
        console.assert(connection.state === signalR.HubConnectionState.Connected);
        console.log('Conectado Hub de identificação');
    } catch (err) {
        console.assert(connection.state === signalR.HubConnectionState.Disconnected); itau
        ////ATENÇÃO!: Logar também na aplicação principal o retorno de erro
        console.log("Erro: HUBIDENT0001 - " + err.toString());
        setTimeout(() => start(), 500000);

    }

};

//Caso conexão feche
connection.onclose(async () => {
    await start();
});

//Função Inicio da conexão
start();

//reconectando a conexão
connection.onreconnecting(error => {
    console.assert(connection.state === signalR.HubConnectionState.Reconnecting);
    console.log('Reconectando ao Hub de identificação' + error);
});

var chartBlock = '\u25A3';
var clientIdToken;

connection.on("IdentifyMessage", function (responseCodeHttp, responseMessage, responseDataRetorn) {

    //Retorno
    const obj = JSON.parse(responseDataRetorn);

    console.log("IdentifyMessage", responseCodeHttp, responseMessage);
    console.log("IdentifyMessageJson", obj);

    //Itens abaixo apenas para incluir em nossa interface de testes
    //Implementar conforme aplicação
    var pollResultMsg = " StatusCode: " + responseCodeHttp + " - Message: " + responseMessage + ". ";
    var ulPoll = document.getElementById("messagesList");
    var liPollResult = document.createElement("li");


    //Guardamos o retorno de clientIdToken,nome e datahoraLGPD na senha que será emitida
    var token = obj.clienteIdToken;
    var nome = obj.nome;
    var prioridade = obj.idPrioridade;
    var categoria = obj.idCategoria;
    var dataHora = obj.dataHora;


    if (token && nome && dataHora) {
        document.getElementById("clientIdToken").value = token;
        document.getElementById("clientNome").value = nome;
        document.getElementById("dataHora").value = dataHora;
        pollResultMsg += "ClienteIdToken: " + token + " Prioridade: " + prioridade + " Categoria: " + categoria
    }

    if (prioridade) {
        prioridade = "prioridade" + prioridade;
        if (prioridade) {
            var radioPrioridade = document.getElementById(prioridade);
            if (radioPrioridade) {
                radioPrioridade.checked = true;
            }
        }
    }
    if (categoria) {
        categoria = "categoria" + categoria;
        if (categoria) {
            var radiocategoria = document.getElementById(categoria);
            if (radiocategoria) {
                radiocategoria.checked = true;
            }
        }        
    }

    liPollResult.textContent = pollResultMsg;
    ulPoll.insertBefore(liPollResult, ulPoll.childNodes[0]);
});


connection.on("UpdateIdentifyMessage", function (responseCodeHttp, responseMessage) {

    //Retorno
    console.log("IdentifyMessage", responseCodeHttp, responseMessage);


    //Itens abaixo apenas para incluir em nossa interface de testes
    //Implementar conforme aplicação
    var pollResultMsg = " StatusCode: " + responseCodeHttp + " - Message: " + responseMessage + ". ";

    //Implementar conforme aplicação
    var ulPoll = document.getElementById("messagesList");
    var liPollResult = document.createElement("li");
    liPollResult.textContent = pollResultMsg;
    ulPoll.insertBefore(liPollResult, ulPoll.childNodes[0]);
});



document.getElementById("validateButton").addEventListener("click", function (event) {
    limparDadosParaIdentificacao();

    //Documento digitado
    var documento = document.getElementById("numDocInput").value;
    var documento = documento.replace(/[^\d]+/g, '')

    //numero da agência configurado
    var agencia = document.getElementById("agenciaInput").value;
    var agencia = agencia.replace(/[^\d]+/g, '')

    //datahora do envio do numero do documento
    var dataHora = new Date();
    
    //Validação dos campos obrigatórios, como ddocumento e agência

    if (documento && agencia) {
        connection.invoke("IdentifyMessage", documento, agencia, dataHora)
            .then(() => console.log)
            .catch(function (err) {
                ////ATENÇÃO!: Logar também na aplicação principal o retorno de erro
                //Continuar para fluxo principal, com a apresentação da tela inicial
                return console.error(err.toString());

            });

    } else {
        //Caso dados não tenham sido encontrados ou limpados para novo cliente
        ////ATENÇÃO!: Logar também na aplicação principal o retorno de erro
        //Continuar para fluxo principal, com a apresentação da tela inicial
        return console.log("Erro: HUBIDENT0003 - Documentos não foram encontrados para realizar identificação do cliente");
    }

    event.preventDefault();

});



document.getElementById("UpdateButton").addEventListener("click", function (event) {


    //numero da agência configurado
    var agencia = document.getElementById("agenciaInput").value;
    var agencia = agencia.replace(/[^\d]+/g, '')

    //datahora da geração da senha
    let datahoraEmissao = new Date();

    //Numero da senha gerada
    var numeroTicket = gerarSenhaAleatoria();

    //Token armazenado na senha do cliente anteriormente identificado
    var token = document.getElementById("clientIdToken").value;
    
    //DataHora armazenado na senha do cliente anteriormente identificado
    var dataHora = document.getElementById("dataHora").value;
    
    if (token && agencia && numeroTicket) {

        connection.invoke("UpdateIdentifyMessage", token, dataHora, agencia, numeroTicket, datahoraEmissao)

            .catch(function (err) {
                ////ATENÇÃO!: Logar também na aplicação principal o retorno de erro
                //Continuar para fluxo principal, com a apresentação da tela inicial
                return console.error(err.toString());
            });



    } else {
        //Caso dados não tenham sido encontrados ou limpados para novo cliente
        //ATENÇÃO!: Logar também na aplicação principal o retorno de erro
        //Continuar para fluxo principal, com a apresentação da tela inicial
        return console.log("Erro: HUBIDENT0003 - Informações da senha não foram encontrados para atualização da identificação do cliente");

    }

    event.preventDefault();

});



//Desconsiderar funções abaixo

//Função para gerar senha aleatoria
function gerarSenhaAleatoria() {

    let letra = Math.random()       // Gera um valor randômico
        .toString(36)                   // Utiliza a Base36
        .substr(-1)                     // Captura os 4 últimos números
        .toUpperCase();                 // Converte para maiúscula    

    let numero = Math.floor((Math.random() * (999 - 100)) + 100); // Gera um valor entre 999 e 1000  
    return letra + numero;

}

// Função para gerar CorrelationId aleatório
function generateCorrelationId() {
    return Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);
}

// Função para gerar CorrelationId aleatório
function generateApikeyId() {
    return "020de488-2aee-42ad-990d-1159fd43d3ea";
}

// Função para gerar CorrelationId aleatório
function generateToken() {
    return "123456";
}

function limparDadosParaIdentificacao() {
    document.getElementById("clientIdToken").value = "";
    document.getElementById("clientNome").value = "";
    document.getElementById("dataHora").value = "";

    document.getElementById("prioridade1").checked = false;
    document.getElementById("prioridade2").checked = false;
    document.getElementById("prioridade3").checked = false;

    document.getElementById("categoria1").checked = false;
    document.getElementById("categoria2").checked = false;
    document.getElementById("categoria3").checked = false;
    document.getElementById("categoria4").checked = false;
    document.getElementById("categoria5").checked = false;

}