# SentimentWatch

A real-time stock sentiment analysis dashboard that combines stock price data with news sentiment analysis.

## Features

- **Real-time Stock Data**: Fetches live stock prices from Alpha Vantage
- **News Aggregation**: Pulls relevant news articles from NewsAPI
- **Sentiment Analysis**: Analyzes news sentiment to gauge market sentiment
- **Interactive Dashboard**: Blazor-based web interface for visualization

## Architecture

- **StockSentiment.Api**: ASP.NET Core Web API backend
- **StockSentiment.Web**: Blazor Server frontend
- **StockSentiment.Core**: Core business logic and interfaces
- **StockSentiment.Infrastructure**: External API integrations
- **StockSentiment.ML**: Machine learning sentiment analysis (future)

## Quick Start

### Prerequisites

- .NET 9.0 SDK
- Docker Desktop (for containerized deployment)
- API Keys:
  - [Alpha Vantage](https://www.alphavantage.co/) (free tier available)
  - [NewsAPI](https://newsapi.org/) (free tier available)

### Local Development (without Docker)

1. **Clone the repository**

2. **Set up User Secrets for API project:**
   ```bash
   cd src/StockSentiment.Api
   dotnet user-secrets set "StockData:ApiKey" "YOUR_ALPHA_VANTAGE_KEY"
   dotnet user-secrets set "NewsData:ApiKey" "YOUR_NEWSAPI_KEY"
   ```

3. **Run the API:**
   ```bash
   dotnet run --project src/StockSentiment.Api
   ```

4. **Run the Web UI (in a new terminal):**
   ```bash
   dotnet run --project src/StockSentiment.Web
   ```

5. **Access the application:**
   - Web UI: https://localhost:7076
   - API: https://localhost:7001

### Docker Deployment

See [DEPLOYMENT.md](DEPLOYMENT.md) for comprehensive Docker and cloud deployment instructions.

**Quick Docker Start:**

1. **Ensure Docker Desktop is running**

2. **Create `.env` file from template:**
   ```bash
   cp .env.example .env
   # Edit .env and add your API keys
   ```

3. **Build and run:**
   ```bash
   docker-compose up --build
   ```

4. **Access:**
   - Web UI: http://localhost:5200
   - API: http://localhost:5100

## Project Structure

```
SentimentWatch/
├── src/
│   ├── StockSentiment.Api/          # REST API
│   ├── StockSentiment.Web/          # Blazor UI
│   ├── StockSentiment.Core/         # Domain models & interfaces
│   ├── StockSentiment.Infrastructure/ # External integrations
│   └── StockSentiment.ML/           # ML sentiment analysis
├── Dockerfile.Api                    # API container definition
├── Dockerfile.Web                    # Web container definition
├── docker-compose.yml                # Multi-container orchestration
├── DEPLOYMENT.md                     # Deployment guide
└── README.md                         # This file
```

## Configuration

### Development (User Secrets)

API keys are stored in User Secrets for local development:

```bash
dotnet user-secrets set "StockData:ApiKey" "your-key"
dotnet user-secrets set "NewsData:ApiKey" "your-key"
```

### Production (Environment Variables)

For Docker and cloud deployments, use environment variables:

```bash
StockData__ApiKey=your-key
NewsData__ApiKey=your-key
```

Note: Use double underscores (`__`) to represent nested JSON configuration.

## API Endpoints

- `GET /api/sentiment/{symbol}` - Get sentiment snapshot for a stock symbol

Example:
```bash
curl http://localhost:5100/api/sentiment/AAPL
```

## Technology Stack

- **Backend**: ASP.NET Core 9.0
- **Frontend**: Blazor Server
- **Containerization**: Docker
- **Data Sources**: Alpha Vantage, NewsAPI
- **Future**: ML.NET for sentiment analysis

## Roadmap

- [x] Core architecture
- [x] Real data integration
- [x] Docker deployment
- [ ] Advanced ML sentiment analysis
- [ ] Historical trend analysis
- [ ] User authentication
- [ ] Watchlist functionality
- [ ] Real-time updates via SignalR

## Contributing

This is a portfolio project. Feel free to fork and adapt for your own use.

## License

MIT License - See LICENSE file for details

## Security

- Never commit API keys to version control
- `.env` files are gitignored
- Use User Secrets for local development
- Use environment variables for production
- Rotate API keys regularly

## Support

For issues or questions, please open an issue on GitHub.
