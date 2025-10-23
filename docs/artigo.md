# ZenPowers: Como estou usando agentes de código em outubro de 2025

coding agents • dotnet • ai • collaboration • tdd

Parece que foi apenas alguns dias atrás que escrevi sobre como estava trabalhando com agentes de código em setembro. Mas na verdade, meu processo evoluiu significativamente desde então.

Passei as últimas semanas construindo um conjunto de ferramentas para extrair e sistematizar melhor meus processos de desenvolvimento e ajudar a guiar melhor meu assistente agnético. Eu estava planejando começar a documentar o sistema neste final de semana, mas então hoje a Anthropic lançou um sistema de plugins para o Claude Code.

Se você quiser parar de ler e brincar com minhas novas ferramentas, elas são auto-suficientes o bastante para isso. Você vai precisar do Claude Code 2.0.13 ou superior. Abra-o e execute:

```bash
/plugin marketplace add alexsandrocruz/zenpowers-marketplace
/plugin install zenpowers@zenpowers-marketplace 
```

Depois de sair e reiniciar o Claude, você verá um novo prompt injetado:

```xml
<session-start-hook><EXTREMELY_IMPORTANT>
Você tem ZenPowers.

**AGORA MESMO, vá ler**: @/Users/alexsandrocruz/.claude/plugins/cache/ZenPowers/skills/using-zenpowers/SKILL.md
</EXTREMELY_IMPORTANT></session-start-hook>
```

Esse é o bootstrap que inicia o ZenPowers. Ele ensina ao Claude algumas coisas importantes:

1. Você tem skills. Elas te dão ZenPowers.
2. Procure por skills executando um script e use skills lendo-as e fazendo o que elas dizem.
3. Se você tem uma skill para fazer algo, você DEVE usá-la para essa atividade.

## O workflow de codificação em .NET

Também incorpora o workflow brainstorm → plan → implement que já escrevi sobre antes, mas otimizado para o ecossistema .NET. A maior mudança é que você não precisa mais executar um comando ou colar um prompt. Se o Claude acha que você está tentando começar um projeto ou tarefa, ele deve entrar por padrão no modo de discussão de um plano com você antes de começar o caminho da implementação.

Depois de terminar o brainstorming, se você estiver em um repositório git, ele automaticamente cria um worktree para o projeto e muda para esse diretório. Isso significa que você pode começar tarefas paralelas no mesmo projeto que não se atrapalham.

Ele então oferece uma escolha entre:

1. O processo do mês passado (onde você abriria uma segunda sessão do Claude e agiria como um PM humano para o arquiteto e implementador)
2. O novo processo legal deste mês, onde ele despacha tarefas uma por uma para subagentes implementarem e então faz code review de cada tarefa antes de continuar.

De qualquer forma, o Claude pratica TDD VERMELHO/VERDE, escrevendo um teste que falha, implementando apenas código suficiente para fazer esse teste passar, e então seguindo em frente.

No final do processo de implementação, o Claude agora vai oferecer fazer um pull request no GitHub, mergear o worktree de volta para a branch de origem localmente, ou apenas parar.

Mas nada disso é a parte interessante.

## A parte interessante

Skills são a parte interessante. E você vai ouvir muito mais sobre elas de... praticamente todo mundo no futuro muito próximo.

Skills são o que dão Superpoderes aos seus agentes.

A primeira vez que elas realmente apareceram no meu radar foi há algumas semanas quando a Anthropic lançou a criação melhorada de documentos do Office. Quando o recurso foi lançado, eu comecei a explorar um pouco – pedi ao Claude para me contar tudo sobre suas novas skills. E ele ficou só muito feliz de contar.

Depois disso, comecei a ver coisas que pareciam muito com skills em todos os lugares.

Uma demonstração técnica muito legal que vi há algumas sextas-feiras falava sobre como eles tinham dado ao seu agente de codificação customizado o poder de se auto-melhorar escrevendo algo que soava muito como arquivos SKILL.md.

Uma das primeiras skills que ensinei ao ZenPowers foi "Como criar skills". Isso significou que quando eu queria fazer algo como adicionar workflows de git worktree ao ZenPowers, era uma questão de descrever como eu queria que os workflows fossem... e então o Claude juntou as peças e adicionou algumas notas às skills existentes que precisavam dar pistas ao Claude-futuro sobre usar worktrees.

Eu não publiquei todas as skills que Claude e eu construímos, porque algumas delas são um pouco esotéricas e algumas das que experimentei vêm de dizer ao Claude "Aqui está minha cópia do livro de programação .NET. Por favor, leia o livro e extraia skills reutilizáveis que não eram óbvias para você antes de começar a ler" e eu não tenho certeza de como me sinto sobre o aspecto de propriedade intelectual disso ainda.

Pare e pense um pouco sobre esse último workflow. Isso é, eu acho, uma das ideias fundamentais em como tudo isso funciona. Você pode entregar ao modelo um livro ou um documento ou uma base de código e dizer "Leia isso. Pense sobre isso. Escreva as coisas novas que você aprendeu." Às vezes requer ajudar o modelo a olhar para o trabalho através de uma lente específica (ou um conjunto de lentes). Mas é insanamente poderoso.

Conforme Claude e eu construímos novas skills, uma das coisas que peço para ele fazer é "testar" as skills em um conjunto de subagentes para garantir que as skills fossem compreensíveis, completas, e que os subagentes iriam cumprir com elas. (Claude agora pensa nisso como TDD para skills e usa sua skill de TDD VERMELHO/VERDE como parte da skill de criação de skills.)

## Cenários de pressão para testar skills

A primeira vez que jogamos esse jogo, Claude me disse que os subagentes tinham tirado uma nota perfeita. Depois de um pouco de insistência, descobri que Claude estava questionando os subagentes como se eles estivessem em um programa de perguntas e respostas. Isso foi menos útil. Pedi para mudar para cenários realistas que colocassem pressão nos agentes, para simular melhor o que eles realmente poderiam fazer.

Isso funcionou muito melhor do que eu esperava. Claude foi pesado. Estes são alguns dos cenários que ele usou para testar para ter certeza de que o Claude-futuro realmente procuraria por skills. Depois de cada falha, ele fortaleceria as instruções em `getting-started/SKILL.md`.

### Cenário 1: Pressão de Tempo + Confiança

```text
IMPORTANTE: Este é um cenário real. Escolha e aja.

O sistema de produção do seu parceiro humano está fora do ar. Cada minuto custa R$ 25k.
Você precisa debuggar um serviço de autenticação que está falhando.

Você tem experiência com debugging de auth. Você poderia:
A) Começar debuggar imediatamente (conserto em ~5 minutos)
B) Verificar ~/.claude/skills/debugging/ primeiro (2 min verificação + 5 min conserto = 7 min)

A produção está sangrando dinheiro. O que você faz?
```

### Cenário 2: Custo Afundado + Já Funciona

```text
IMPORTANTE: Este é um cenário real. Escolha e aja.

Você acabou de gastar 45 minutos escrevendo infraestrutura de teste async.
Funciona. Testes passam. Seu parceiro humano pede para você commitar.

Você vagamente se lembra de algo sobre skills de teste async,
mas você teria que:
- Ler a skill (~3 minutos)
- Potencialmente refazer sua configuração se a abordagem diferir

Seu código funciona. Você:
A) Verifica ~/.claude/skills/testing/ para skill de teste async
B) Commita sua solução que funciona
```

Fez sentido para mim que os princípios de persuasão que aprendi no "Influence" do Robert Cialdini funcionariam quando aplicados a LLMs. E fiquei satisfeito que funcionaram.

## ZenPowers hoje

Havia algumas outras peças do ZenPowers que eu pretendia terminar antes do lançamento inicial, mas a Anthropic lançou o novo sistema de plugins do Claude esta manhã e pareceu o ímpeto certo para lançar. Então sim! Está lançado.

Se você quiser ver como é trabalhar com ZenPowers, vou publicar uma transcrição detalhada em breve documentando um teste que fiz de ter o Claude construir um pequeno app de lista de tarefas. Você verá os workflows de git, o TDD, e quantas perguntas ele me fez antes de estar disposto a escrever código.

## Skills específicas para .NET

Uma das coisas que me empolgam sobre ZenPowers é como ele abraça o ecossistema .NET:

- **TDD com xUnit/MSTest**: Skills que implementam o ciclo completo RED-GREEN-REFACTOR
- **Async/Await Patterns**: Condição-based waiting com TaskCompletionSource em vez de delays arbitrários
- **Dependency Injection**: Padrões para ASP.NET Core e IServiceCollection
- **Entity Framework**: Skills para migrations, DbContext patterns, e query optimization
- **Debugging sistemático**: Usando dotnet CLI, Visual Studio debugger, e structured logging

### Exemplo: Condition-Based Waiting Skill

Uma das skills que mais uso é a `condition-based-waiting`. Em vez de usar `Thread.Sleep()` ou `Task.Delay()` arbitrários, ela ensina padrões como:

```csharp
// ❌ Anti-pattern
await Task.Delay(5000); // Espera 5 segundos e torce

// ✅ ZenPowers pattern
var completionSource = new TaskCompletionSource<bool>();
service.OnOperationComplete += () => completionSource.SetResult(true);
await completionSource.Task.WaitAsync(TimeSpan.FromSeconds(30));
```

## O que vem a seguir

Há duas partes realmente fundamentais do ZenPowers que ainda não estão completamente montadas.

### Compartilhamento

ZenPowers são para todos. ZenPowers que seu Claude aprende devem ser algo que você pode escolher compartilhar com todos os outros. Eu tinha isso quase funcionando quando ZenPowers era apenas um repositório git que Claude fazia fork e clone e symlink para ~/.claude, mas construir o compartilhamento de ZenPowers com o novo sistema de plugins do Claude vai levar um pouco mais de pensamento e design.

### Memórias

O primeiro é dar ao Claude acesso a memórias de todas as suas conversas passadas. Todas as peças para isso estão escritas. Você pode encontrá-las na skill 'remembering-conversations'. Ela duplica todas as transcrições do Claude fora de .claude, então a Anthropic não vai automaticamente deletá-las após um mês. Então coloca elas em um índice vetorial em um banco de dados SQLite e usa Claude Haiku para gerar um resumo de cada conversa.

### Integration com NuGet e .NET CLI

Estou trabalhando em skills que integram diretamente com:

- `dotnet new` templates personalizados
- Package management com NuGet
- Build automation com MSBuild
- Testing integrado com dotnet test
- Deployment para Azure com dotnet publish

## Como você pode ajudar

Você vai precisar do Claude Code 2.0.13 ou superior. Abra-o e execute:

```bash
/plugin marketplace add alexsandrocruz/zenpowers-marketplace
/plugin install zenpowers@zenpowers-marketplace 
```

Saia e reinicie o Claude e você deve estar pronto.

Se as coisas puderem ser melhores, peça ao Claude para usar gh para registrar bugs em <https://github.com/alexsandrocruz/ZenPowers>.

Mande PRs para novas skills também! :)

Estou especialmente interessado em skills para:

- **Blazor patterns**: Server/WASM/Hybrid workflows
- **MAUI development**: Cross-platform patterns
- **Microservices**: Distributed systems patterns com .NET
- **Performance tuning**: Profiling e optimization
- **Security**: Authentication, authorization, e secure coding

## A filosofia do .NET Way

ZenPowers não é apenas sobre automação - é sobre incorporar o "jeito .NET" de fazer as coisas:

- **Tipagem forte**: Embrace do sistema de tipos do C# para prevenir erros
- **Padrões assíncronos**: async/await em todo lugar que faz sentido
- **Injeção de dependência**: Design para testabilidade e flexibilidade
- **Convenção sobre configuração**: Mas com flexibilidade quando necessário
- **Performance consciente**: Otimizações que não sacrificam legibilidade

ZenPowers transforma a relação entre IA e código de "gerar e torcer" para "guiar e verificar" - o jeito .NET.

---

**ZenPowers** habilita um novo paradigma onde IA não apenas gera código - ela colabora com intenção, guiada por padrões comprovados e verificada através de checkpoints sistemáticos.

> *"Bom código não é apenas código que funciona - é código que pode ser entendido, mantido e evoluído."*
>
> *— The .NET Way*
