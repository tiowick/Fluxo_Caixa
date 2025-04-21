# Decisão Arquitetural: Monolito Modular

## Justificativa

Para o desafio proposto, optei por uma arquitetura **monolítica modular**. Essa escolha foi motivada pelos seguintes fatores:

- **Simplicidade e Manutenção:** Um monolito modular é mais simples de desenvolver, testar e manter, especialmente em projetos de pequeno e médio porte ou MVPs.
- **Separação de Domínios:** O projeto foi estruturado em camadas e módulos bem definidos (Domínio, Aplicação, Infraestrutura), facilitando a manutenção e evolução.
- **Resiliência:** Os serviços de Lançamentos e Consolidação são desacoplados logicamente. Caso o serviço de consolidação falhe, o serviço de lançamentos continua funcionando normalmente, garantindo disponibilidade.
- **Escalabilidade:** A arquitetura permite evolução para microserviços no futuro, caso a demanda cresça. As dependências são desacopladas por meio de interfaces, facilitando a extração de módulos independentes.
- **Desempenho e Testabilidade:** Menor overhead de comunicação entre serviços, maior performance inicial e facilidade para implementar testes automatizados.

## Como atender aos requisitos do desafio

- **Escalabilidade:**
  - O sistema pode ser escalado horizontalmente como monolito (várias instâncias).
  - Caso necessário, módulos como Lançamentos e Consolidação podem ser extraídos como microserviços no futuro.
- **Resiliência:**
  - Uso de padrões como Retry Policy e Circuit Breaker para lidar com falhas.
  - Serviços desacoplados: falha em um não derruba o outro.
- **Segurança:**
  - Facilidade de implementar autenticação/autorização centralizada.
  - Possibilidade de evoluir para mecanismos mais robustos em microserviços.
- **Integração:**
  - Interfaces e contratos bem definidos entre módulos.
  - Facilidade para integrar outros sistemas futuramente.

## Evolução futura: De monolito para microserviços

Caso a aplicação cresça e a demanda por escalabilidade ou independência de deploy aumente, a arquitetura permite:

1. **Extração de módulos:** Separar os módulos de Lançamentos e Consolidação em microserviços independentes.
2. **Comunicação via mensageria:** Utilizar filas (ex: RabbitMQ, Azure Service Bus) para desacoplar ainda mais os domínios.
3. **Escalabilidade independente:** Cada serviço pode ser escalado conforme a demanda.
4. **Autenticação distribuída:** Implementar OAuth2/JWT para autenticação entre serviços.

## Diagrama de Arquitetura

![Diagrama de Arquitetura](docs/diagrama-arquitetura.png)

---

> **Resumo:**
> A escolha do monolito modular atende plenamente aos requisitos do desafio, com separação clara de responsabilidades, resiliência, escalabilidade e facilidade de evolução futura para microserviços.
