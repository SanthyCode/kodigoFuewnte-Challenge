import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

// 1. Creamos la interfaz para los items (Igual a tu SeparataItemDto en C#)
export interface SeparataItemRequest {
  productId: string;
  promotionType: string;
  promotionValue: number;
}

// 2. Actualizamos la interfaz principal para incluir la lista de items
export interface SeparataRequest {
  name: string;
  startDate: string;
  endDate: string;
  items: SeparataItemRequest[]; // ¡Aquí está el eslabón perdido!
}

export interface ValidationResponse {
  isValid: boolean;
  errors: string[];
}

export interface SeparataResponse {
  mensaje: string;
  data?: unknown;
}

// Creamos el servicio que conectará con nuestro backend
export const separataApi = createApi({
  reducerPath: 'separataApi',
  baseQuery: fetchBaseQuery({ baseUrl: 'http://127.0.0.1:5000/api/' }),
  endpoints: (builder) => ({
    validateSeparata: builder.mutation<ValidationResponse, SeparataRequest>({
      query: (separata) => ({
        url: 'Separata/validate',
        method: 'POST',
        body: separata,
      }),
    }),
    
    // 2. Reemplaza el 'any' por 'SeparataResponse' aquí:
    createSeparata: builder.mutation<SeparataResponse, SeparataRequest>({
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