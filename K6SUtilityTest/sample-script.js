import http from 'k6/http';
import { sleep } from 'k6';
import * as config from './config.js';

export default function() {
    http.get(config.RANDOM_NUMBER_ENDPOINT);
    sleep(1);
    http.get(config.RANDOM_NUMBER_WITH_DELAY_ENDPOINT);
    sleep(1);
    http.get(config.STRING_REVERSE_ENDPOINT.replace('{str}', 'sample'));
    sleep(1);
}