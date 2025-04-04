import { execSync } from 'node:child_process';
import { join } from 'node:path';

// Monta o caminho absoluto da pasta Configurations
const configPath = join(process.cwd(), 'Configurations');

// Comando final com path correto
const command = `dotnet tool run t4 -P "${configPath}" ./Configurations/AppSettings.tt`;

try {
    console.log(`Executando: ${command}`);
    execSync(command, { stdio: 'inherit' });
} catch (error) {
    console.error('Erro ao executar o comando T4:');
    console.error(error.message);
    process.exit(1);
}
