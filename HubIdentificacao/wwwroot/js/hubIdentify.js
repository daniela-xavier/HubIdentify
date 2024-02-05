"use strict";


//configurações da conexão
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/identifyClient")
    .configureLogging(signalR.LogLevel.Information)
    .build();


//Função Inicio da conexão

async function start() {

    try {
        await connection.start().catch(err => console.error(err.toString()));
        console.assert(connection.state === signalR.HubConnectionState.Connected);
        console.log('Conectado Hub de identificação');
    } catch (err) {
        console.assert(connection.state === signalR.HubConnectionState.Disconnected);itau
        ////ATENÇÃO!: Logar também na aplicação principal o retorno de erro
        console.log("Erro: HUBIDENT0004 - " + err.toString());
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
    console.log("IdentifyMessage", responseCodeHttp, responseMessage);
    console.log("IdentifyMessageData", responseDataRetorn);
    const obj = JSON.parse(responseDataRetorn);

    //Itens abaixo apenas para incluir em nossa interface de testes

    //Implementar conforme aplicação

    var pollResultMsg = " StatusCode: "+ responseCodeHttp +" - Message:"+ responseMessage;
    var ulPoll = document.getElementById("messagesList");
    var liPollResult = document.createElement("li");
    
    
    //Guardamos o retorno de clientIdToken,nome e datahoraLGPD na senha que será emitida
    var token = obj.data.clienteIdToken;
    var nome = obj.data.nome;
    var dataHora = obj.data.dataHora;
    var prioridade = obj.data.idPrioridade;
    var categoria = obj.data.idCategoria;


    if(token && nome && dataHora){
        document.getElementById("clientIdToken").value = token;
        document.getElementById("clientNome").value = nome;
        document.getElementById("dataHoraLGPD").value = dataHora;
        pollResultMsg += "ClienteIdToken: " + token +"Prioridade: " + prioridade +"Categoria: " + categoria
    }
    
    liPollResult.textContent = pollResultMsg;
    ulPoll.insertBefore(liPollResult, ulPoll.childNodes[0]);


});



connection.on("UpdateIdentifyMessage", function (request, response) {

    //Retorno
    console.log("UpdateIdentifyMessage", request, response);

    //Itens abaixo apenas para incluir em nossa interface de testes
    //Implementar conforme aplicação
    var pollResultMsg = response + "'.";
    //Implementar conforme aplicação
    var ulPoll = document.getElementById("messagesList");
    var liPollResult = document.createElement("li");
    liPollResult.textContent = pollResultMsg;
    ulPoll.insertBefore(liPollResult, ulPoll.childNodes[0]);
});



document.getElementById("validateButton").addEventListener("click", function (event) {

    //Documento digitado
    var documento = document.getElementById("numDocInput").value;
    var documento = documento.replace(/[^\d]+/g, '')

    //numero da agência configurado
    var agencia = document.getElementById("agenciaInput").value;
    var agencia = agencia.replace(/[^\d]+/g, '')



    //datahora do envio do numero do documento
    var dataHoraLGPD = new Date();

    //Validação dos campos obrigatórios, como ddocumento e agência

    if (documento && agencia) {
        connection.invoke("IdentifyMessage", documento, agencia, dataHoraLGPD)
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
        return console.log("Erro: HUBIDENT0001 - Documentos não foram encontrados para realizar identificação do cliente");
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
    var clientIdToken = document.getElementById("clientIdToken").value;

    //DataHora armazenado na senha do cliente anteriormente identificado
    var dataHoraLGPD = document.getElementById("dataHoraLGPD").value;

    if (clientIdToken && agencia && numeroTicket) {

        connection.invoke("UpdateIdentifyMessage", clientIdToken, dataHoraLGPD, agencia, numeroTicket, datahoraEmissao)

            .catch(function (err) {
                ////ATENÇÃO!: Logar também na aplicação principal o retorno de erro
                //Continuar para fluxo principal, com a apresentação da tela inicial
                return console.error(err.toString());
            });



    } else {
        //Caso dados não tenham sido encontrados ou limpados para novo cliente
        //ATENÇÃO!: Logar também na aplicação principal o retorno de erro
        //Continuar para fluxo principal, com a apresentação da tela inicial
        return console.log("Erro: HUBIDENT0002 - Informações da senha não foram encontrados para atualização da identificação do cliente");

    }

    event.preventDefault();

});



//Desconsiderar função

function gerarSenhaAleatoria() {

    let letra = Math.random()       // Gera um valor randômico
        .toString(36)                   // Utiliza a Base36
        .substr(-1)                     // Captura os 4 últimos números
        .toUpperCase();                 // Converte para maiúscula    

    let numero = Math.floor((Math.random() * (999 - 100)) + 100); // Gera um valor entre 999 e 1000  
    return letra + numero;

}