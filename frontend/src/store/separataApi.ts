import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

// Definimos los tipos de datos (como las interfaces en C#)
export interface SeparataRequest {
  name: string;
  startDate: string;
  endDate: string;
}

export interface ValidationResponse {
  isValid: boolean;
  errors: string[];
}

// Creamos el servicio que conectará con nuestro backend
export const separataApi = createApi({
  reducerPath: 'separataApi',
  baseQuery: fetchBaseQuery({ baseUrl: 'http://127.0.0.1:5000/api/' }),
  endpoints: (builder) => ({
    
    // Endpoint para validar en tiempo real
    validateSeparata: builder.mutation<ValidationResponse, SeparataRequest>({
      query: (separata) => ({
        url: 'Separata/validate',
        method: 'POST',
        body: separata,
      }),
    }),

    // Endpoint para guardar la oferta
    createSeparata: builder.mutation<any, SeparataRequest>({
      query: (separata) => ({
        url: 'Separata',
        method: 'POST',
        body: separata,
      }),
    }),
    
  }),
});

// Redux Toolkit genera Hooks automáticamente basados en los nombres de los endpoints
export const { useValidateSeparataMutation, useCreateSeparataMutation } = separataApi;