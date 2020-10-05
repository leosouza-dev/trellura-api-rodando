using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Trellura.API.Data;
using Trellura.Models;

namespace Trellura.Hubs
{
  public class TrelluraHub : Hub
  {
    public TrelluraHub(TrelluraDbContext context)
    {
      _context = context;
    }

    private TrelluraDbContext _context;
    private static int _totalDeUsuarios;
    private string _usuario;
    private static HashSet<string> _usuariosDoGrupo = new HashSet<string>{};
    private static string _nomeDogrupo = "GrupoTrellura";
    private static List<Cartao> _listaDeCartoes = new List<Cartao>();

    private static IDictionary<string, string> _dicionarioUsuariosGrupo = new Dictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
      _totalDeUsuarios++;
      await Clients.All.SendAsync("atualizarTotalUsuarios", _totalDeUsuarios);
      await base.OnConnectedAsync();
    }

    // É preciso mostrar esse método - se ficar atualizando a pagina e
    // esse método não estiver implementado
    // fica incrementando o numero de usuário...
    public async override Task OnDisconnectedAsync(Exception exception)
    {
        _totalDeUsuarios--;
        _dicionarioUsuariosGrupo.Remove(Context.ConnectionId);

        var listaDeUsuariosNoGrupo = new List<string>();
        //tualiza lista para ser enviada ao cliente
        foreach (var usuario in _dicionarioUsuariosGrupo)
        {
            listaDeUsuariosNoGrupo.Add(usuario.Value);
        }


        await Groups.RemoveFromGroupAsync(Context.ConnectionId, _nomeDogrupo);

        await Clients.Group(_nomeDogrupo).SendAsync("entrandoNoGrupo", listaDeUsuariosNoGrupo); // alterar o nome para atualiza usuarios

        await Clients.All.SendAsync("atualizarTotalUsuarios", _totalDeUsuarios); // atualiza na home
        await base.OnDisconnectedAsync(exception);
    }

    public async Task Entrar(string usuario)
    {
        if(string.IsNullOrEmpty(usuario))
        {
            await Clients.Caller.SendAsync("erroMensagem", "Digite um nome para entrar no grupo!");
            return;
        } 

        _usuario = usuario;
        _dicionarioUsuariosGrupo.Add(Context.ConnectionId, _usuario);
        //_usuariosDoGrupo.Add(usuario);

        var listaDeUsuariosNoGrupo = new List<string>();
        //tualiza lista para ser enviada ao cliente
        foreach (var nome in _dicionarioUsuariosGrupo)
        {
            listaDeUsuariosNoGrupo.Add(nome.Value);
        }

        _listaDeCartoes = await _context.Cartoes.ToListAsync();

        await Groups.AddToGroupAsync(Context.ConnectionId, _nomeDogrupo);
            //await Clients.Group(_nomeDogrupo).SendAsync("entrandoNoGrupo", _usuariosDoGrupo);
            await Clients.Group(_nomeDogrupo).SendAsync("entrandoNoGrupo", listaDeUsuariosNoGrupo);
            await Clients.Caller.SendAsync("entrouNoGrupo", _listaDeCartoes);
        return;
    }

    public async Task CriarCard(string tituloCartao)
    {
        // comparado ao modelState
        if(string.IsNullOrEmpty(tituloCartao))
        {
            await Clients.Caller.SendAsync("exibeMensagemErro", "Título é obirgatório!"); // no formulário envia mensagem com sucesso
            return;
        }
        else
        {
            try
            {
                Cartao cartao = new Cartao(tituloCartao);
                
                _context.Cartoes.Add(cartao);
                await _context.SaveChangesAsync();

                _listaDeCartoes = await _context.Cartoes.ToListAsync();

                // await Clients.Caller.SendAsync("cardCriado", "Card criado com sucesso!"); // no formulário envia mensagem com sucesso
                // await Clients.GroupExcept(_nomeDogrupo, Context.ConnectionId).SendAsync("atualizarBoard", cartao); // para o resto atualiza o board
                await Clients.Group(_nomeDogrupo).SendAsync("atualizarBoard", _listaDeCartoes);
            }
            catch (Exception)
            { 
                await Clients.Caller.SendAsync("exibeMensagemErro", "Não foi possível criar o Card");
            }
        }
    }

    public async Task AtualizarCartao(string id, string titulo, string status)
    {
        var idSelecionado = Convert.ToInt32(id);
        var statusSelecionado = Convert.ToInt32(status);

            if (!CardExiste(idSelecionado))
        {
            await Clients.Caller.SendAsync("exibeMensagemErro", "Card Não encontrado - checar Id"); // no formulário envia mensagem com sucesso
            return;
        }
        else
        {
            try
            {
                var cartao = await _context.Cartoes.FirstOrDefaultAsync(c => c.Id == idSelecionado);
                cartao.Status = (StatusDoCartao)statusSelecionado;
                cartao.Titulo = titulo;

                _context.Update(cartao);
                await _context.SaveChangesAsync();

                _listaDeCartoes = await _context.Cartoes.ToListAsync();
                await Clients.Group(_nomeDogrupo).SendAsync("atualizarBoard", _listaDeCartoes);
                // await Clients.Caller.SendAsync("cardAtualizado", "Card atualizado com sucesso!"); // no formulário envia mensagem com sucesso
                // await Clients.GroupExcept(_nomeDogrupo, Context.ConnectionId).SendAsync("atualizarBoard", card); // para o resto atualiza o board
            }
            catch (DbUpdateConcurrencyException)
            {
                await Clients.Caller.SendAsync("exibeMensagemErro", "Não foi possível atualizar o Card. Tente novamente mais tarde");
            }
        }

    }

    public async Task ApagarCartao(string id)
    {

        var idSelecionado = Convert.ToInt32(id);

        // TODO atulizar verificando se o id existe na lista (criar um método)
        if (!CardExiste(idSelecionado))
        {
            await Clients.Caller.SendAsync("exibeMensagemErro", "Card Não encontrado - checar Id"); // no formulário envia mensagem com sucesso
            return;
        }

        try
        {
            var cartao = await _context.Cartoes.FirstOrDefaultAsync(c => c.Id == idSelecionado);
            _context.Remove(cartao);
            await _context.SaveChangesAsync();

            _listaDeCartoes = await _context.Cartoes.ToListAsync();

            await Clients.Group(_nomeDogrupo).SendAsync("atualizarBoard", _listaDeCartoes);
        }
        catch (Exception)
        {
            await Clients.Caller.SendAsync("exibeMensagemErro", "Não foi possível apagar o Card. Tente novamente mais tarde");
        }
    }

        private bool CardExiste(int id)
        {
            return _context.Cartoes.Any(e => e.Id == id);
        }
  }
}