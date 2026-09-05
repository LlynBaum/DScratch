import { defineConfig } from 'vitest/config';
import { playwright } from '@vitest/browser-playwright';

export default defineConfig({
    test: {
        browser: {
            provider: playwright({
                launchOptions: {
                    executablePath: '/usr/bin/chromium-browser',
                },
            }),
            enabled: true,
            instances: [
                { browser: 'chromium' },
            ],
            headless: true,
            locators: {
                testIdAttribute: 'data-dnode-id',
            },
        },
        setupFiles: ['./tests/setup.ts'],
        include: ['**/*.test.ts'],
        testTimeout: 2000
    }
});